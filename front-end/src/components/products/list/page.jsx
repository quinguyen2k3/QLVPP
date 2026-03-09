import * as React from 'react';
import * as XLSX from 'xlsx';
import {
  TableContainer,
  Table,
  TableRow,
  TableCell,
  TableBody,
  Avatar,
  Typography,
  TableHead,
  Chip,
  Box,
  MenuItem,
  Button,
  Divider,
  IconButton,
  Grid,
  Stack,
} from '@mui/material';
import DownloadCard from 'src/components/shared/DownloadCard';
import CustomTextField from 'src/components/forms/theme-elements/CustomTextField';
import CustomSelect from 'src/components/forms/theme-elements/CustomSelect';

import {
  flexRender,
  getCoreRowModel,
  getFilteredRowModel,
  getPaginationRowModel,
  getSortedRowModel,
  useReactTable,
  createColumnHelper,
} from '@tanstack/react-table';
import { Link } from 'react-router-dom';
import {
  IconChevronLeft,
  IconChevronRight,
  IconChevronsLeft,
  IconChevronsRight,
  IconEdit,
} from '@tabler/icons-react';
import { useTranslation } from 'react-i18next';

// Thay thế hàm fetch cũ bằng useMasterData
import { useMasterData } from 'src/hooks/useMasterData';
import EmptyImage from 'src/assets/images/svgs/no-data.webp';

const getImageUrl = (path) => {
  if (!path) return null;
  if (path.startsWith('http') || path.startsWith('data:')) return path;
  let baseURL = import.meta.env.VITE_API_BASE_URL || '';
  baseURL = baseURL.replace(/\/api\/?$/, '');
  return `${baseURL}${path.startsWith('/') ? '' : '/'}${path}`;
};

function Filter({ column, t }) {
  const columnFilterValue = column.getFilterValue();
  const { filterVariant, filterOptions } = column.columnDef.meta || {};

  if (filterVariant === 'select') {
    return (
      <CustomSelect
        value={columnFilterValue ?? ''}
        onChange={(e) => column.setFilterValue(e.target.value)}
        fullWidth
      >
        <MenuItem value="">{t('Placeholder.AllStatus') || 'All'}</MenuItem>
        {filterOptions
          ? filterOptions.map((opt) => (
              <MenuItem key={opt.value} value={opt.value}>
                {opt.label}
              </MenuItem>
            ))
          : [
              <MenuItem key="true-opt" value="true">
                {t('Status.Active')} / {t('Option.Yes')}
              </MenuItem>,
              <MenuItem key="false-opt" value="false">
                {t('Status.Inactive')} / {t('Option.No')}
              </MenuItem>,
            ]}
      </CustomSelect>
    );
  }

  return (
    <CustomTextField
      placeholder={t('Placeholder.Search')}
      value={columnFilterValue || ''}
      onChange={(e) => column.setFilterValue(e.target.value)}
      fullWidth
    />
  );
}

const columnHelper = createColumnHelper();

const ProductListTable = () => {
  const { t } = useTranslation();
  
  // Lấy toàn bộ dữ liệu từ useMasterData, set default là mảng rỗng để chống lỗi crash UI
  const { 
    products = [], 
    categories = [], 
    units = [], 
    loading: masterLoading 
  } = useMasterData();

  const [columnFilters, setColumnFilters] = React.useState([]);

  const categoryMap = React.useMemo(
    () => new Map(categories.map((c) => [c.id, c.name])),
    [categories],
  );

  const unitMap = React.useMemo(
    () => new Map(units.map((u) => [u.id, u.name])), 
    [units]
  );

  const columns = React.useMemo(
    () => [
      columnHelper.accessor('imagePath', {
        header: '',
        enableSorting: false,
        enableColumnFilter: false,
        cell: (info) => {
          const url = getImageUrl(info.getValue());
          return (
            <Avatar
              src={url || '/fallback-image.png'}
              variant="rounded"
              sx={{ width: 50, height: 50 }}
            />
          );
        },
      }),
      columnHelper.accessor('prodCode', { header: t('Field.Code') }),
      columnHelper.accessor('name', { header: t('Field.Name') }),
      columnHelper.accessor('categoryId', {
        header: t('Menu.Category'),
        cell: (info) => categoryMap.get(info.getValue()) || t('Placeholder.Unknown'),
        meta: {
          filterVariant: 'select',
          filterOptions: categories.map((c) => ({ label: c.name, value: c.id })),
        },
        filterFn: (row, columnId, filterValue) => {
          if (!filterValue) return true;
          return row.getValue(columnId) === filterValue;
        },
      }),
      columnHelper.accessor('unitId', {
        header: t('Menu.Unit'),
        cell: (info) => unitMap.get(info.getValue()) || t('Placeholder.Unknown'),
        meta: {
          filterVariant: 'select',
          filterOptions: units.map((u) => ({ label: u.name, value: u.id })),
        },
        filterFn: (row, columnId, filterValue) => {
          if (!filterValue) return true;
          return row.getValue(columnId) === filterValue;
        },
      }),
      columnHelper.accessor('isAsset', {
        header: t('Field.Asset'),
        cell: (info) => (
          <Chip
            label={info.getValue() ? t('Option.Yes') : t('Option.No')}
            color={info.getValue() ? 'primary' : 'default'}
            size="small"
          />
        ),
        meta: { filterVariant: 'select' },
        filterFn: (row, columnId, filterValue) => {
          if (filterValue === undefined || filterValue === '') return true;
          return String(row.getValue(columnId)) === String(filterValue);
        },
      }),
      columnHelper.accessor('isActivated', {
        header: t('Field.Status'),
        cell: (info) => (
          <Chip
            label={info.getValue() ? t('Status.Active') : t('Status.Inactive')}
            color={info.getValue() ? 'success' : 'error'}
            size="small"
          />
        ),
        meta: { filterVariant: 'select' },
        filterFn: (row, columnId, filterValue) => {
          if (filterValue === undefined || filterValue === '') return true;
          return String(row.getValue(columnId)) === String(filterValue);
        },
      }),
      columnHelper.display({
        id: 'actions',
        header: '',
        cell: (info) => (
          <Button
            component={Link}
            to={`/catalog/product/edit/${info.row.original.id}`}
            color="primary"
            size="small"
            sx={{ minWidth: 0, padding: '6px' }}
          >
            <IconEdit width={18} />
          </Button>
        ),
        enableSorting: false,
        enableColumnFilter: false,
      }),
    ],
    [categories, units, categoryMap, unitMap, t],
  );

  const table = useReactTable({
    data: products, // Truyền thẳng mảng products từ hook vào
    columns,
    state: { columnFilters },
    onColumnFiltersChange: setColumnFilters,
    getCoreRowModel: getCoreRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
  });

  const handleDownload = () => {
    const dataToExport = products.map((product) => ({
      [t('Field.Code')]: product.prodCode,
      [t('Field.Name')]: product.name,
      [t('Menu.Category')]: categoryMap.get(product.categoryId) || '',
      [t('Menu.Unit')]: unitMap.get(product.unitId) || '',
      [t('Field.Asset')]: product.isAsset ? t('Option.Yes') : t('Option.No'),
      [t('Field.Status')]: product.isActivated ? t('Status.Active') : t('Status.Inactive'),
    }));

    const worksheet = XLSX.utils.json_to_sheet(dataToExport);
    const workbook = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(workbook, worksheet, 'Product List');
    XLSX.writeFile(workbook, 'product-list.xlsx');
  };

  return (
    <DownloadCard title={t('Page.ProductList')} onDownload={handleDownload}>
      <Grid container spacing={3}>
        <Grid item size={12}>
          <Box>
            <TableContainer>
              <Table sx={{ whiteSpace: 'nowrap' }}>
                <TableHead>
                  {table.getHeaderGroups().map((headerGroup) => (
                    <TableRow key={headerGroup.id}>
                      {headerGroup.headers.map((header) => (
                        <TableCell key={header.id}>
                          <Typography
                            variant="h6"
                            mb={1}
                            className={
                              header.column.getCanSort() ? 'cursor-pointer select-none' : ''
                            }
                            onClick={header.column.getToggleSortingHandler()}
                          >
                            {header.isPlaceholder
                              ? null
                              : flexRender(header.column.columnDef.header, header.getContext())}
                            {(() => {
                              const sortState = header.column.getIsSorted();
                              if (sortState === 'asc') return ' 🔼';
                              if (sortState === 'desc') return ' 🔽';
                              return null;
                            })()}
                          </Typography>
                          {header.column.getCanFilter() && <Filter column={header.column} t={t} />}
                        </TableCell>
                      ))}
                    </TableRow>
                  ))}
                </TableHead>
                <TableBody>
                  {/* Có thể bổ sung loading indicator ở đây nếu masterLoading là true */}
                  {table.getRowModel().rows.length === 0 ? (
                    <TableRow>
                      <TableCell colSpan={columns.length}>
                        <Box
                          sx={{
                            display: 'flex',
                            flexDirection: 'column',
                            alignItems: 'center',
                            justifyContent: 'center',
                            height: 240,
                          }}
                        >
                          <img
                            src={EmptyImage}
                            alt={t('Message.NoData')}
                            style={{ width: 180, marginBottom: 12 }}
                          />
                          <Typography variant="body1" color="text.secondary">
                            {t('Message.NoData')}
                          </Typography>
                        </Box>
                      </TableCell>
                    </TableRow>
                  ) : (
                    table.getRowModel().rows.map((row) => (
                      <TableRow key={row.id}>
                        {row.getVisibleCells().map((cell) => (
                          <TableCell key={cell.id}>
                            {flexRender(cell.column.columnDef.cell, cell.getContext())}
                          </TableCell>
                        ))}
                      </TableRow>
                    ))
                  )}
                </TableBody>
              </Table>
            </TableContainer>
            <Divider />
            <Stack gap={1} p={3} alignItems="center" direction="row" justifyContent="space-between">
              <Box display="flex" alignItems="center" gap={1}>
                <Typography variant="body1">
                  {table.getPrePaginationRowModel().rows.length} {t('Field.Rows') || 'Rows'}
                </Typography>
              </Box>
              <Box display="flex" alignItems="center" gap={1}>
                <Stack direction="row" alignItems="center" gap={1}>
                  <Typography variant="body1">{t('Field.Page') || 'Page'}</Typography>
                  <Typography variant="body1" fontWeight={600}>
                    {table.getState().pagination.pageIndex + 1} {t('Field.Of') || 'of'}{' '}
                    {table.getPageCount()}
                  </Typography>
                </Stack>
                <Stack direction="row" alignItems="center" gap={1}>
                  | {t('Action.GoToPage') || 'Go to page'}:
                  <CustomTextField
                    type="number"
                    min="1"
                    max={table.getPageCount()}
                    defaultValue={table.getState().pagination.pageIndex + 1}
                    onChange={(e) => {
                      const page = e.target.value ? Number(e.target.value) - 1 : 0;
                      table.setPageIndex(page);
                    }}
                  />
                </Stack>
                <CustomSelect
                  value={table.getState().pagination.pageSize}
                  onChange={(e) => {
                    table.setPageSize(Number(e.target.value));
                  }}
                >
                  {[10, 15, 20, 25].map((pageSize) => (
                    <MenuItem key={pageSize} value={pageSize}>
                      {pageSize}
                    </MenuItem>
                  ))}
                </CustomSelect>

                <IconButton
                  size="small"
                  onClick={() => table.setPageIndex(0)}
                  disabled={!table.getCanPreviousPage()}
                >
                  <IconChevronsLeft />
                </IconButton>
                <IconButton
                  size="small"
                  onClick={() => table.previousPage()}
                  disabled={!table.getCanPreviousPage()}
                >
                  <IconChevronLeft />
                </IconButton>
                <IconButton
                  size="small"
                  onClick={() => table.nextPage()}
                  disabled={!table.getCanNextPage()}
                >
                  <IconChevronRight />
                </IconButton>
                <IconButton
                  size="small"
                  onClick={() => table.setPageIndex(table.getPageCount() - 1)}
                  disabled={!table.getCanNextPage()}
                >
                  <IconChevronsRight />
                </IconButton>
              </Box>
            </Stack>
          </Box>
        </Grid>
      </Grid>
    </DownloadCard>
  );
};

export default ProductListTable;