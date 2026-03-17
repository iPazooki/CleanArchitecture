import type {
  HTMLAttributes,
  ReactNode,
  TableHTMLAttributes,
  TdHTMLAttributes,
} from "react";

interface TableProps extends TableHTMLAttributes<HTMLTableElement> {
  children: ReactNode;
}

interface TableHeaderProps extends HTMLAttributes<HTMLTableSectionElement> {
  children: ReactNode;
}

interface TableBodyProps extends HTMLAttributes<HTMLTableSectionElement> {
  children: ReactNode;
}

interface TableRowProps extends HTMLAttributes<HTMLTableRowElement> {
  children: ReactNode;
}

interface TableCellProps extends TdHTMLAttributes<HTMLTableCellElement> {
  children: ReactNode;
  isHeader?: boolean;
}

const Table = ({ children, className = "", ...props }: TableProps) => {
  return (
    <table className={`min-w-full ${className}`.trim()} {...props}>
      {children}
    </table>
  );
};

const TableHeader = ({ children, className, ...props }: TableHeaderProps) => {
  return (
    <thead className={className} {...props}>
      {children}
    </thead>
  );
};

const TableBody = ({ children, className, ...props }: TableBodyProps) => {
  return (
    <tbody className={className} {...props}>
      {children}
    </tbody>
  );
};

const TableRow = ({ children, className, ...props }: TableRowProps) => {
  return (
    <tr className={className} {...props}>
      {children}
    </tr>
  );
};

const TableCell = ({ children, isHeader = false, className = "", ...props }: TableCellProps) => {
  const CellTag = isHeader ? "th" : "td";

  return (
    <CellTag className={className.trim()} {...props}>
      {children}
    </CellTag>
  );
};

export { Table, TableHeader, TableBody, TableRow, TableCell };
