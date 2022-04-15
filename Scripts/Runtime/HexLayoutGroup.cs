/// Copyright (c) Matt Pewsey, All Rights Reserved.

using UnityEngine;
using UnityEngine.UI;

namespace MPewsey.HexagonalUI
{
    /// <summary>
    /// Attach this component to a GameObject to apply hexagonal layout
    /// to its children. For buttons used as children, use the HexButton
    /// component in lieu of the built in Button component for more
    /// intuitive input navigation.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class HexLayoutGroup : LayoutGroup
    {
        [SerializeField]
        private float _circumradius = 50.0f;
        /// <summary>
        /// The circumradius of the hexagonal elements.
        /// </summary>
        public float Circumradius
        {
            get => _circumradius;
            set => SetProperty(ref _circumradius, value);
        }

        [SerializeField]
        private Vector2 _spacing = new Vector2(20.0f, 20.0f);
        /// <summary>
        /// The horizontal and vertical spacing between hexagonal elements.
        /// </summary>
        public Vector2 Spacing
        {
            get => _spacing;
            set => SetProperty(ref _spacing, value);
        }

        [SerializeField]
        private Axis _startAxis;
        /// <summary>
        /// The start axis for tiling elements.
        /// </summary>
        public Axis StartAxis
        {
            get => _startAxis;
            set => SetProperty(ref _startAxis, value);
        }

        [SerializeField]
        private Axis _cellOrientation;
        /// <summary>
        /// The orientation of the hexagonal element long axes.
        /// </summary>
        public Axis CellOrientation
        {
            get => _cellOrientation;
            set => SetProperty(ref _cellOrientation, value);
        }

        [SerializeField]
        private ConstraintType _constraint;
        /// <summary>
        /// The constraint used for paging elements.
        /// </summary>
        public ConstraintType Constraint
        {
            get => _constraint;
            set => SetProperty(ref _constraint, value);
        }

        [SerializeField]
        private int _constraintCount = 10;
        /// <summary>
        /// The constaint count for non-flexible constraints.
        /// </summary>
        public int ConstraintCount
        {
            get => _constraintCount;
            set => SetProperty(ref _constraintCount, Mathf.Max(value, 1));
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            ConstraintCount = Mathf.Max(ConstraintCount, 1);
        }
#endif

        /// <summary>
        /// Calculates the minimum and preferred widths for the object.
        /// </summary>
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
            var totalMin = Mathf.Max(padding.horizontal + ContentWidth(MinColumnCount()), 0.0f);
            var totalPreferred = Mathf.Max(padding.horizontal + ContentWidth(PreferredColumnCount()), 0.0f);
            SetLayoutInputForAxis(totalMin, totalPreferred, -1.0f, 0);
        }

        /// <summary>
        /// Calculates the minimum and preferred heights for the object.
        /// </summary>
        public override void CalculateLayoutInputVertical()
        {
            var totalMin = Mathf.Max(padding.vertical + ContentHeight(MinRowCount()), 0.0f);
            var totalPreferred = Mathf.Max(padding.vertical + ContentHeight(PreferredRowCount()), 0.0f);
            SetLayoutInputForAxis(totalMin, totalPreferred, -1.0f, 1);
        }

        /// <summary>
        /// Sets the cell sizes for the hexagonal elements.
        /// </summary>
        public override void SetLayoutHorizontal()
        {
            SetCellSizes();
        }

        /// <summary>
        /// Sets the positions of the hexagonal elements.
        /// </summary>
        public override void SetLayoutVertical()
        {
            CalculateLayout();
        }

        /// <summary>
        /// Returns true if the value is even.
        /// </summary>
        private static bool IsEven(int value)
        {
            return (value & 1) == 0;
        }

        /// <summary>
        /// Sets the cell sizes for the hexagonal elements.
        /// </summary>
        private void SetCellSizes()
        {
            var cellSize = CellSize();

            for (int i = 0; i < rectChildren.Count; i++)
            {
                var rectChild = rectChildren[i];
                m_Tracker.Add(this, rectChild, DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.SizeDelta);
                rectChild.anchorMin = Vector2.up;
                rectChild.anchorMax = Vector2.up;
                rectChild.sizeDelta = cellSize;
            }
        }

        /// <summary>
        /// Returns the content height for the specified number of rows.
        /// </summary>
        private float ContentHeight(int rows)
        {
            if (rows <= 0)
            {
                return 0.0f;
            }

            float value;
            var cellSize = CellSize();

            if (CellOrientation == Axis.Horizontal)
            {
                value = rows * (cellSize.y + Spacing.y) - Spacing.y + 0.5f * (cellSize.y + Spacing.y);
            }
            else
            {
                value = rows * (0.75f * cellSize.y + Spacing.y) - Spacing.y + 0.25f * cellSize.y;
            }

            return Mathf.Max(value, 0.0f);
        }

        /// <summary>
        /// Returns the content width for the specified number of columns.
        /// </summary>
        private float ContentWidth(int columns)
        {
            if (columns <= 0)
            {
                return 0.0f;
            }

            float value;
            var cellSize = CellSize();

            if (CellOrientation == Axis.Horizontal)
            {
                value = columns * (0.75f * cellSize.x + Spacing.x) - Spacing.x + 0.25f * cellSize.x;
            }
            else
            {
                value = columns * (cellSize.x + Spacing.x) - Spacing.x + 0.5f * (cellSize.x + Spacing.x);
            }

            return Mathf.Max(value, 0.0f);
        }

        /// <summary>
        /// Sets the positions of the hexagonal elements.
        /// </summary>
        private void CalculateLayout()
        {
            var rows = RowCount();
            var columns = ColumnCount();
            var cellSize = CellSize();
            var x0 = GetStartOffset(0, ContentWidth(columns));
            var y0 = GetStartOffset(1, ContentHeight(rows));

            for (int i = 0; i < rectChildren.Count; i++)
            {
                int row, column;
                float x, y;

                if (StartAxis == Axis.Horizontal)
                {
                    row = i / columns;
                    column = i % columns;
                }
                else
                {
                    row = i % rows;
                    column = i / rows;
                }

                if (CellOrientation == Axis.Horizontal)
                {
                    x = x0 + column * (0.75f * cellSize.x + Spacing.x);
                    y = y0 + row * (cellSize.y + Spacing.y);

                    if (IsEven(column))
                    {
                        y += 0.5f * (cellSize.y + Spacing.y);
                    }
                }
                else
                {
                    x = x0 + column * (cellSize.x + Spacing.x);
                    y = y0 + row * (0.75f * cellSize.y + Spacing.y);

                    if (IsEven(row))
                    {
                        x += 0.5f * (cellSize.x + Spacing.x);
                    }
                }

                SetChildAlongAxis(rectChildren[i], 0, x, cellSize.x);
                SetChildAlongAxis(rectChildren[i], 1, y, cellSize.y);
            }
        }

        /// <summary>
        /// Returns the row count used for the minimum layout height.
        /// </summary>
        private int MinRowCount()
        {
            switch (Constraint)
            {
                case ConstraintType.FixedRow:
                    return ConstraintCount;
                case ConstraintType.FixedColumn:
                    return Mathf.Max(1, (rectChildren.Count - 1) / ConstraintCount + 1);
                default:
                    return 1;
            }
        }

        /// <summary>
        /// Returns the row count used for the preferred layout height.
        /// </summary>
        private int PreferredRowCount()
        {
            switch (Constraint)
            {
                case ConstraintType.FixedRow:
                    return ConstraintCount;
                case ConstraintType.FixedColumn:
                    return Mathf.Max(1, (rectChildren.Count - 1) / ConstraintCount + 1);
                default:
                    return Mathf.CeilToInt(Mathf.Sqrt(rectChildren.Count));
            }
        }

        /// <summary>
        /// Returns the column count used for the minimum layout width.
        /// </summary>
        private int MinColumnCount()
        {
            switch (Constraint)
            {
                case ConstraintType.FixedColumn:
                    return ConstraintCount;
                case ConstraintType.FixedRow:
                    return Mathf.Max(1, (rectChildren.Count - 1) / ConstraintCount + 1);
                default:
                    return 1;
            }
        }

        /// <summary>
        /// Returns the column count used for the preferred layout width.
        /// </summary>
        private int PreferredColumnCount()
        {
            switch (Constraint)
            {
                case ConstraintType.FixedColumn:
                    return ConstraintCount;
                case ConstraintType.FixedRow:
                    return Mathf.Max(1, (rectChildren.Count - 1) / ConstraintCount + 1);
                default:
                    return Mathf.CeilToInt(Mathf.Sqrt(rectChildren.Count));
            }
        }

        /// <summary>
        /// Returns the current row count.
        /// </summary>
        private int RowCount()
        {
            return RowCount(rectChildren.Count);
        }

        /// <summary>
        /// Returns the row count given a total element count.
        /// </summary>
        public int RowCount(int count)
        {
            var constraint = Mathf.Min(GetConstraintCount(), count);

            if (constraint > 0 && !ConstraintIsRowCount())
            {
                return (count - 1) / constraint + 1;
            }

            return constraint;
        }

        /// <summary>
        /// Returns the current column count.
        /// </summary>
        private int ColumnCount()
        {
            return ColumnCount(rectChildren.Count);
        }

        /// <summary>
        /// Returns the column count given a total element count.
        /// </summary>
        public int ColumnCount(int count)
        {
            var constraint = Mathf.Min(GetConstraintCount(), count);

            if (constraint > 0 && ConstraintIsRowCount())
            {
                return (count - 1) / constraint + 1;
            }

            return constraint;
        }

        /// <summary>
        /// Returns the hexagonal element width and height.
        /// </summary>
        public Vector2 CellSize()
        {
            var longDiagonal = 2.0f * Circumradius;
            var shortDiagonal = 0.86602540378f * longDiagonal;

            if (CellOrientation == Axis.Horizontal)
            {
                return new Vector2(longDiagonal, shortDiagonal);
            }

            return new Vector2(shortDiagonal, longDiagonal);
        }

        /// <summary>
        /// Returns true if the controlling constraint is the row count.
        /// Otherwise, the controlling constraint is the column count.
        /// </summary>
        public bool ConstraintIsRowCount()
        {
            if (Constraint == ConstraintType.Flexible)
            {
                return StartAxis == Axis.Vertical;
            }

            return Constraint == ConstraintType.FixedRow;
        }

        /// <summary>
        /// Returns the controlling constraint count.
        /// </summary>
        public int GetConstraintCount()
        {
            if (Constraint == ConstraintType.Flexible)
            {
                return FlexibleConstraintCount();
            }

            return ConstraintCount;
        }

        /// <summary>
        /// Returns the constraint count for the flexible constraint.
        /// </summary>
        private int FlexibleConstraintCount()
        {
            if (StartAxis == Axis.Horizontal)
            {
                return FlexibleWidthConstraintCount();
            }

            return FlexibleHeightConstraintCount();
        }

        /// <summary>
        /// Returns the constraint count for the flexible width constraint.
        /// </summary>
        private int FlexibleWidthConstraintCount()
        {
            float value;
            var cellSize = CellSize();
            var size = rectTransform.rect.size;

            if (CellOrientation == Axis.Horizontal)
            {
                value = (size.x + Spacing.x - 0.25f * cellSize.x - padding.horizontal) / (0.75f * cellSize.x + Spacing.x);
            }
            else
            {
                value = (size.x + Spacing.x - 0.5f * (cellSize.x + Spacing.x) - padding.horizontal) / (cellSize.x + Spacing.x);
            }

            return Mathf.Max(Mathf.FloorToInt(value), 1);
        }

        /// <summary>
        /// Returns the constraint count for the flexible height constraint.
        /// </summary>
        private int FlexibleHeightConstraintCount()
        {
            float value;
            var cellSize = CellSize();
            var size = rectTransform.rect.size;

            if (CellOrientation == Axis.Horizontal)
            {
                value = (size.y + Spacing.y - 0.5f * (cellSize.y + Spacing.y) - padding.vertical) / (cellSize.y + Spacing.y);
            }
            else
            {
                value = (size.y + Spacing.y - 0.25f * cellSize.y - padding.vertical) / (0.75f * cellSize.y + Spacing.y);
            }

            return Mathf.Max(Mathf.FloorToInt(value), 1);
        }

        public enum Axis
        {
            Horizontal = 0,
            Vertical = 1,
        }

        public enum ConstraintType
        {
            Flexible,
            FixedRow,
            FixedColumn,
        }
    }
}
