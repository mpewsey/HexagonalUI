/// Copyright (c) Matt Pewsey, All Rights Reserved.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MPewsey.HexagonalUI
{
    /// <summary>
    /// This component overrides the built in Button component's navigation to work
    /// more intuitively with the staggered element stacking of the HexLayoutGroup.
    /// This component should be used instead of the built in Button component
    /// for buttons that are immediate children of a HexLayoutGroup.
    /// </summary>
    public class HexButton : Button
    {
        /// <summary>
        /// A list of sibling selectables populated during a navigation query.
        /// </summary>
        private static List<Selectable> SiblingSelectables { get; set; } = new List<Selectable>();

        /// <summary>
        /// Returns the axis for the specified direction.
        /// </summary>
        private static int GetAxis(Vector3 direction)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                return 0;
            return 1;
        }

        /// <summary>
        /// Returns the selectable on navigation up.
        /// </summary>
        public override Selectable FindSelectableOnUp()
        {
            if (navigation.mode == Navigation.Mode.Explicit)
                return navigation.selectOnUp;
            if ((navigation.mode & Navigation.Mode.Vertical) != Navigation.Mode.None)
                return GetSelectable(transform.rotation * Vector3.up);
            return null;
        }

        /// <summary>
        /// Returns the selectable on navigation down.
        /// </summary>
        public override Selectable FindSelectableOnDown()
        {
            if (navigation.mode == Navigation.Mode.Explicit)
                return navigation.selectOnDown;
            if ((navigation.mode & Navigation.Mode.Vertical) != Navigation.Mode.None)
                return GetSelectable(transform.rotation * Vector3.down);
            return null;
        }

        /// <summary>
        /// Returns the selectable on navigation left.
        /// </summary>
        public override Selectable FindSelectableOnLeft()
        {
            if (navigation.mode == Navigation.Mode.Explicit)
                return navigation.selectOnLeft;
            if ((navigation.mode & Navigation.Mode.Horizontal) != Navigation.Mode.None)
                return GetSelectable(transform.rotation * Vector3.left);
            return null;
        }

        /// <summary>
        /// Returns the selectable on navigation right.
        /// </summary>
        public override Selectable FindSelectableOnRight()
        {
            if (navigation.mode == Navigation.Mode.Explicit)
                return navigation.selectOnRight;
            if ((navigation.mode & Navigation.Mode.Horizontal) != Navigation.Mode.None)
                return GetSelectable(transform.rotation * Vector3.right);
            return null;
        }

        /// <summary>
        /// Sets the sibling selectables array with all active selectables.
        /// </summary>
        private void SetSiblingSelectables()
        {
            SiblingSelectables.Clear();
            var parent = transform.parent;

            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);

                if (child.gameObject.activeInHierarchy)
                {
                    var selectable = child.GetComponent<Selectable>();
                    SiblingSelectables.Add(selectable);
                }
            }
        }

        /// <summary>
        /// Returns the selectable in the specified direction.
        /// </summary>
        public Selectable GetSelectable(Vector3 direction)
        {
            var result = FindSelectable(direction);

            if (result == null || transform.parent == null || !IsActive())
                return result;

            var hexLayout = transform.parent.GetComponent<HexLayoutGroup>();
            var directionAxis = GetAxis(direction);

            if (hexLayout == null || directionAxis != (int)hexLayout.CellOrientation)
                return result;

            SetSiblingSelectables();
            var startAxis = (int)hexLayout.StartAxis;
            var rows = hexLayout.RowCount(SiblingSelectables.Count);
            var columns = hexLayout.ColumnCount(SiblingSelectables.Count);
            var index = GetIndex(rows, columns, startAxis);
            var (row, column) = (index.x, index.y);
            Selectable selectable;

            if (row < 0 || column < 0)
                return result;

            if (directionAxis == 0)
            {
                if (direction.x > 0)
                    selectable = GetSelectableOnRight(row, column, rows, columns, startAxis);
                else
                    selectable = GetSelectableOnLeft(row, column, rows, columns, startAxis);
            }
            else
            {
                if (direction.y > 0)
                    selectable = GetSelectableOnUp(row, column, rows, columns, startAxis);
                else
                    selectable = GetSelectableOnDown(row, column, rows, columns, startAxis);
            }

            SiblingSelectables.Clear();
            return selectable != null ? selectable : result;
        }

        /// <summary>
        /// Returns the next active selectable above the specified row-column index.
        /// </summary>
        private Selectable GetSelectableOnUp(int row, int column, int rows, int columns, int startAxis)
        {
            for (int i = row - 1; i >= 0; i--)
            {
                var index = GetFlatIndex(i, column, rows, columns, startAxis);

                if (CanNavigateTo(SiblingSelectables[index]))
                    return SiblingSelectables[index];
            }

            return null;
        }

        /// <summary>
        /// Returns the next active selectable below the specified row-column index.
        /// </summary>
        private Selectable GetSelectableOnDown(int row, int column, int rows, int columns, int startAxis)
        {
            for (int i = row + 1; i < rows; i++)
            {
                var index = GetFlatIndex(i, column, rows, columns, startAxis);

                if (index < SiblingSelectables.Count && CanNavigateTo(SiblingSelectables[index]))
                    return SiblingSelectables[index];
            }

            return null;
        }

        /// <summary>
        /// Returns the next active selectable to the right of specified row-column index.
        /// </summary>
        private Selectable GetSelectableOnRight(int row, int column, int rows, int columns, int startAxis)
        {
            for (int i = column + 1; i < columns; i++)
            {
                var index = GetFlatIndex(row, i, rows, columns, startAxis);

                if (index < SiblingSelectables.Count && CanNavigateTo(SiblingSelectables[index]))
                    return SiblingSelectables[index];
            }

            return null;
        }

        /// <summary>
        /// Returns the next active selectable to the left of the specified row-column index.
        /// </summary>
        private Selectable GetSelectableOnLeft(int row, int column, int rows, int columns, int startAxis)
        {
            for (int i = column - 1; i >= 0; i--)
            {
                var index = GetFlatIndex(row, i, rows, columns, startAxis);

                if (CanNavigateTo(SiblingSelectables[index]))
                    return SiblingSelectables[index];
            }

            return null;
        }

        /// <summary>
        /// Returns true if the selectable can be navigated to.
        /// </summary>
        private bool CanNavigateTo(Selectable selectable)
        {
            return selectable != null
                && selectable != this
                && selectable.IsInteractable()
                && selectable.IsActive()
                && selectable.navigation.mode != Navigation.Mode.None;
        }

        /// <summary>
        /// Returns the flat index based on the 
        /// </summary>
        private static int GetFlatIndex(int row, int column, int rows, int columns, int startAxis)
        {
            if (startAxis == 0)
                return row * columns + column;

            return column * rows + row;
        }

        /// <summary>
        /// Returns the row-column index of the button based on its flat
        /// index in the sibling selectables list.
        /// </summary>
        private Vector2Int GetIndex(int rows, int columns, int startAxis)
        {
            for (int i = 0; i < SiblingSelectables.Count; i++)
            {
                if (SiblingSelectables[i] == this)
                {
                    if (startAxis == 0)
                        return new Vector2Int(i / columns, i % columns);

                    return new Vector2Int(i % rows, i / rows);
                }
            }

            return new Vector2Int(-1, -1);
        }
    }
}
