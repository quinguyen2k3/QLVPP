import React, { useContext, useState, useEffect, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Table,
  TableHead,
  TableRow,
  TableCell,
  TableBody,
  TablePagination,
  TextField,
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Tooltip,
  IconButton,
  Box,
  Typography,
  Stack,
  InputAdornment,
  Chip,
  Menu,
  MenuItem,
  CircularProgress,
} from '@mui/material';
import { Grid } from '@mui/material';
import {
  IconEdit,
  IconEye,
  IconListDetails,
  IconSearch,
  IconTrash,
  IconCheck,
  IconX,
  IconClock,
  IconFilter,
  IconChevronDown,
  IconRotateClockwise,
} from '@tabler/icons-react';
import { Link } from 'react-router-dom';
import CustomCheckbox from 'src/components/forms/theme-elements/CustomCheckbox';
import { MaterialRequestContext } from '../../../../context/MaterialRequestContext';
import { useMasterData } from 'src/hooks/useMasterData';
import EmptyImage from 'src/assets/images/svgs/no-data.webp';
import { useTranslation } from 'react-i18next';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { format } from 'date-fns';
import RequireRole from 'src/components/guard';

const MaterialRequestList = () => {
  const navigate = useNavigate();
  const { t } = useTranslation();
  const {
    materialRequests,
    loading,
    deleteMaterialRequest,
    fetchMyRequests,
    fetchToApproveByDepartment,
    fetchToApproveByWarehouse,
    fetchApprovedByWarehouse,
  } = useContext(MaterialRequestContext);

  const { requesters, warehouses } = useMasterData();

  const [searchTerm, setSearchTerm] = useState('');
  const [activeTab, setActiveTab] = useState('All');
  const [selectedItems, setSelectedItems] = useState([]);
  const [selectAll, setSelectAll] = useState(false);

  const [openDeleteDialog, setOpenDeleteDialog] = useState(false);
  const [deleteId, setDeleteId] = useState(null);

  const [viewMode, setViewMode] = useState('MyRequest');
  const [anchorEl, setAnchorEl] = useState(null);
  const openMenu = Boolean(anchorEl);

  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(5);

  const [startDate, setStartDate] = useState(null);
  const [endDate, setEndDate] = useState(null);

  const isFiltering = searchTerm.length > 0 || startDate !== null || endDate !== null;

  const requesterLookup = useMemo(() => {
    return (requesters || []).reduce((acc, cur) => {
      acc[cur.id] = cur.name;
      return acc;
    }, {});
  }, [requesters]);

  const warehouseLookup = useMemo(() => {
    return (warehouses || []).reduce((acc, cur) => {
      acc[cur.id] = cur.name;
      return acc;
    }, {});
  }, [warehouses]);

  useEffect(() => {
    const loadData = () => {
      switch (viewMode) {
        case 'ToApprove':
          fetchToApproveByDepartment();
          break;
        case 'ToApproveWarehouse':
          fetchToApproveByWarehouse();
          break;
        case 'WarehouseApproved':
          fetchApprovedByWarehouse();
          break;
        case 'MyRequest':
        default:
          fetchMyRequests();
          break;
      }
    };
    loadData();
  }, [viewMode]);

  const handleClickMenu = (event) => {
    setAnchorEl(event.currentTarget);
  };

  const handleCloseMenu = () => {
    setAnchorEl(null);
  };

  const handleSelectViewMode = (mode) => {
    setViewMode(mode);
    setPage(0);
    handleCloseMenu();
  };

  const handleResetFilters = () => {
    setSearchTerm('');
    setStartDate(null);
    setEndDate(null);
  };

  const isPendingStatus = (status) => {
    status = status?.toLowerCase() || '';
    return status === 'pending_department' || status === 'pending_warehouse';
  };

  const filteredRequests = (materialRequests || []).filter((item) => {
    const keyword = searchTerm.toLowerCase();
    const requesterName = (requesterLookup[item.requesterId] || '').toLowerCase();
    const code = item.code?.toLowerCase() || '';
    const matchSearch = requesterName.includes(keyword) || code.includes(keyword);

    let matchStatus = true;
    if (activeTab === 'Pending') matchStatus = isPendingStatus(item.status);
    else if (activeTab === 'Approved') matchStatus = item.status === 'Approved';
    else if (activeTab === 'Rejected') matchStatus = item.status === 'Rejected';

    let matchDate = true;
    if (startDate || endDate) {
      if (!item.createdDate) return false;

      const itemDate = new Date(item.createdDate);
      itemDate.setHours(0, 0, 0, 0);

      if (startDate) {
        const start = new Date(startDate);
        start.setHours(0, 0, 0, 0);
        if (itemDate < start) matchDate = false;
      }

      if (endDate) {
        const end = new Date(endDate);
        end.setHours(0, 0, 0, 0);
        if (itemDate > end) matchDate = false;
      }
    }

    return matchSearch && matchStatus && matchDate;
  });

  const isEmpty = filteredRequests.length === 0;

  const paginatedRequests = filteredRequests.slice(
    page * rowsPerPage,
    page * rowsPerPage + rowsPerPage,
  );

  const Pending = filteredRequests.filter((s) => isPendingStatus(s.status)).length;
  const Approved = filteredRequests.filter((s) => s.status === 'Approved').length;
  const Rejected = filteredRequests.filter((s) => s.status === 'Rejected').length;

  const toggleSelectAll = () => {
    const value = !selectAll;
    setSelectAll(value);
    setSelectedItems(value ? filteredRequests.map((s) => s.id) : []);
  };

  const toggleSelectItem = (id) => {
    setSelectedItems((prev) => (prev.includes(id) ? prev.filter((i) => i !== id) : [...prev, id]));
  };

  const handleConfirmDelete = async () => {
    if (deleteId !== null) {
      await deleteMaterialRequest(deleteId);
      if (selectedItems.includes(deleteId)) {
        setSelectedItems((prev) => prev.filter((id) => id !== deleteId));
      }
    } else {
      for (const id of selectedItems) {
        await deleteMaterialRequest(id);
      }
      setSelectedItems([]);
      setSelectAll(false);
    }
    setDeleteId(null);
    setOpenDeleteDialog(false);
  };

  const handleCloseDeleteDialog = () => {
    setOpenDeleteDialog(false);
    setDeleteId(null);
  };

  const getViewLabel = () => {
    switch (viewMode) {
      case 'ToApprove':
        return t('Field.PendingToApproveByDepartment') || 'Chờ trưởng phòng duyệt';
      case 'ToApproveWarehouse':
        return t('Field.PendingToApproveByWarehouse') || 'Chờ thủ kho duyệt';
      case 'WarehouseApproved':
        return t('Field.CompletedApprove') || 'Đã duyệt xong trên kho';
      case 'MyRequest':
      default:
        return t('Field.MyRequests') || 'Yêu cầu của tôi';
    }
  };

  const getStatusLabel = (status) => {
    if (!status) return '';
    status = status?.toLowerCase() || '';
    if (status === 'pending_department') return t('Status.Pending_Department') || 'Chờ TP duyệt';
    if (status === 'pending_warehouse') return t('Status.Pending_Warehouse') || 'Chờ Kho duyệt';
    if (status === 'approved') return t('Status.Approved') || 'Đã duyệt';
    if (status === 'rejected') return t('Status.Rejected') || 'Từ chối';
    return status;
  };

  const getStatusColor = (status) => {
    if (!status) return 'default';
    status = status?.toLowerCase() || '';
    if (status.includes('pending')) return 'warning';
    if (status === 'approved') return 'success';
    if (status === 'rejected') return 'error';
    return 'default';
  };

  return (
    <LocalizationProvider dateAdapter={AdapterDateFns}>
      <Box>
        <Grid container spacing={3}>
          <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
            <Box
              bgcolor="primary.light"
              p={3}
              onClick={() => setActiveTab('All')}
              sx={{ cursor: 'pointer' }}
            >
              <Stack direction="row" gap={2} alignItems="center">
                <Box
                  width={38}
                  height={38}
                  bgcolor="primary.main"
                  display="flex"
                  alignItems="center"
                  justifyContent="center"
                >
                  <IconListDetails color="white" />
                </Box>
                <Box>
                  <Typography>{t('Field.AllRecords') || 'Tất cả'}</Typography>
                  <Typography fontWeight={500}>{filteredRequests.length}</Typography>
                </Box>
              </Stack>
            </Box>
          </Grid>

          <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
            <Box
              bgcolor="warning.light"
              p={3}
              onClick={() => setActiveTab('Pending')}
              sx={{ cursor: 'pointer' }}
            >
              <Stack direction="row" gap={2} alignItems="center">
                <Box
                  width={38}
                  height={38}
                  bgcolor="warning.main"
                  display="flex"
                  alignItems="center"
                  justifyContent="center"
                >
                  <IconClock color="white" />
                </Box>
                <Box>
                  <Typography>{t('Status.Pending') || 'Chờ duyệt'}</Typography>
                  <Typography fontWeight={500}>{Pending}</Typography>
                </Box>
              </Stack>
            </Box>
          </Grid>

          <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
            <Box
              bgcolor="success.light"
              p={3}
              onClick={() => setActiveTab('Approved')}
              sx={{ cursor: 'pointer' }}
            >
              <Stack direction="row" gap={2} alignItems="center">
                <Box
                  width={38}
                  height={38}
                  bgcolor="success.main"
                  display="flex"
                  alignItems="center"
                  justifyContent="center"
                >
                  <IconCheck color="white" />
                </Box>
                <Box>
                  <Typography>{t('Status.Approved') || 'Đã duyệt'}</Typography>
                  <Typography fontWeight={500}>{Approved}</Typography>
                </Box>
              </Stack>
            </Box>
          </Grid>

          <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
            <Box
              bgcolor="error.light"
              p={3}
              onClick={() => setActiveTab('Rejected')}
              sx={{ cursor: 'pointer' }}
            >
              <Stack direction="row" gap={2} alignItems="center">
                <Box
                  width={38}
                  height={38}
                  bgcolor="error.main"
                  display="flex"
                  alignItems="center"
                  justifyContent="center"
                >
                  <IconX color="white" />
                </Box>
                <Box>
                  <Typography>{t('Status.Rejected') || 'Từ chối'}</Typography>
                  <Typography fontWeight={500}>{Rejected}</Typography>
                </Box>
              </Stack>
            </Box>
          </Grid>
        </Grid>

        <Stack
          mt={3}
          direction={{ xs: 'column', md: 'row' }}
          spacing={2}
          justifyContent="space-between"
          alignItems="center"
        >
          <Box display="flex" gap={2} flexGrow={1} flexWrap="wrap" alignItems="center">
            <TextField
              size="small"
              placeholder={t('Placeholder.Search') || 'Tìm kiếm...'}
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <IconSearch size={16} />
                  </InputAdornment>
                ),
              }}
              sx={{ width: { xs: '100%', sm: '250px' } }}
            />

            <DatePicker
              label={t('Field.FromDate') || 'Từ ngày'}
              value={startDate}
              onChange={(newValue) => setStartDate(newValue)}
              slotProps={{
                textField: { size: 'small', sx: { width: { xs: '100%', sm: '150px' } } },
              }}
            />
            <DatePicker
              label={t('Field.ToDate') || 'Đến ngày'}
              value={endDate}
              onChange={(newValue) => setEndDate(newValue)}
              slotProps={{
                textField: { size: 'small', sx: { width: { xs: '100%', sm: '150px' } } },
              }}
            />

            {isFiltering && (
              <Tooltip title={t('Action.ClearFilter') || 'Xóa bộ lọc'}>
                <IconButton onClick={handleResetFilters} color="secondary">
                  <IconRotateClockwise size={20} />
                </IconButton>
              </Tooltip>
            )}
          </Box>

          <Box display="flex" gap={1} alignItems="center">
            <Box>
              <Button
                variant="outlined"
                color="inherit"
                onClick={handleClickMenu}
                startIcon={<IconFilter size={18} />}
                endIcon={<IconChevronDown size={18} />}
                size="medium"
                sx={{ height: '36px', minWidth: '160px', justifyContent: 'space-between' }}
              >
                {getViewLabel()}
              </Button>
              <Menu
                anchorEl={anchorEl}
                open={openMenu}
                onClose={handleCloseMenu}
                anchorOrigin={{ vertical: 'bottom', horizontal: 'left' }}
                transformOrigin={{ vertical: 'top', horizontal: 'left' }}
              >
                <MenuItem
                  onClick={() => handleSelectViewMode('MyRequest')}
                  selected={viewMode === 'MyRequest'}
                >
                  {t('Field.MyRequests') || 'Yêu cầu của tôi'}
                </MenuItem>
                
                <RequireRole allowedRoles={['Department Head']}>
                  <MenuItem
                    onClick={() => handleSelectViewMode('ToApprove')}
                    selected={viewMode === 'ToApprove'}
                  >
                    {t('Field.PendingToApproveByDepartment') || 'Chờ trưởng phòng duyệt'}
                  </MenuItem>
                </RequireRole>

                <RequireRole allowedRoles={['Warehouse Keeper']}>
                  <MenuItem
                    onClick={() => handleSelectViewMode('ToApproveWarehouse')}
                    selected={viewMode === 'ToApproveWarehouse'}
                  >
                    {t('Field.PendingToApproveByWarehouse') || 'Chờ thủ kho duyệt'}
                  </MenuItem>
                  <MenuItem
                    onClick={() => handleSelectViewMode('WarehouseApproved')}
                    selected={viewMode === 'WarehouseApproved'}
                  >
                    {t('Field.CompletedApprove') || 'Đã duyệt xong trên kho'}
                  </MenuItem>
                </RequireRole>
              </Menu>
            </Box>
            
            <RequireRole allowedRoles={['Regular User', 'Department Head']}>
              {selectAll && selectedItems.length > 0 && (
                <Button
                  color="error"
                  variant="outlined"
                  onClick={() => {
                    setDeleteId(null);
                    setOpenDeleteDialog(true);
                  }}
                  startIcon={<IconTrash />}
                  sx={{ height: '36px' }}
                >
                  {t('Action.Delete') || 'Xóa'}
                </Button>
              )}

              <Button
                variant="contained"
                component={Link}
                to="/request/material/add"
                sx={{ height: '36px' }}
              >
                {t('Action.Add') || 'Thêm mới'}
              </Button>
            </RequireRole>
          </Box>
        </Stack>

        <Box mt={2} sx={{ overflowX: 'auto' }}>
          {loading ? (
            <Box display="flex" justifyContent="center" alignItems="center" height="200px">
              <CircularProgress />
            </Box>
          ) : (
            <>
              <Table>
                <TableHead>
                  <TableRow>
                    <TableCell padding="checkbox">
                      <CustomCheckbox
                        checked={selectAll && !isEmpty}
                        disabled={isEmpty}
                        onChange={toggleSelectAll}
                      />
                    </TableCell>
                    <TableCell>{t('Field.Code') || 'Mã phiếu'}</TableCell>
                    <TableCell>{t('Entity.Requester') || 'Người tạo'}</TableCell>
                    <TableCell>{t('Field.Warehouse') || 'Kho yêu cầu'}</TableCell>
                    <TableCell>{t('Field.Date') || 'Ngày yêu cầu'}</TableCell>
                    <TableCell>{t('Field.Status') || 'Trạng thái'}</TableCell>
                    <TableCell align="center">{t('Field.Action') || 'Thao tác'}</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {isEmpty ? (
                    <TableRow>
                      <TableCell colSpan={7}>
                        <Box
                          sx={{
                            display: 'flex',
                            alignItems: 'center',
                            justifyContent: 'center',
                            height: '200px',
                          }}
                        >
                          <img
                            src={EmptyImage}
                            alt={t('Message.NoData')}
                            style={{ maxWidth: '100%', maxHeight: '100%' }}
                          />
                        </Box>
                      </TableCell>
                    </TableRow>
                  ) : (
                    paginatedRequests.map((item) => (
                      <TableRow key={item.id}>
                        <TableCell padding="checkbox">
                          <CustomCheckbox
                            checked={selectedItems.includes(item.id)}
                            onChange={() => toggleSelectItem(item.id)}
                          />
                        </TableCell>
                        <TableCell>
                          <Typography variant="subtitle2" fontWeight={600}>
                            {item.code || `MR-${item.id}`}
                          </Typography>
                        </TableCell>
                        <TableCell>
                          <Typography variant="body2">
                            {requesterLookup[item.requesterId] || 'N/A'}
                          </Typography>
                        </TableCell>
                        <TableCell>
                          <Typography variant="body2">
                            {warehouseLookup[item.warehouseId] || 'N/A'}
                          </Typography>
                        </TableCell>

                        <TableCell>
                          {item.createdDate ? format(new Date(item.createdDate), 'dd/MM/yyyy') : ''}
                        </TableCell>

                        <TableCell>
                          <Chip
                            size="small"
                            label={getStatusLabel(item.status)}
                            color={getStatusColor(item.status)}
                          />
                        </TableCell>
                        <TableCell align="center">
                          <Stack direction="row" justifyContent="center" spacing={1}>
                            <Tooltip title={t('Action.View') || 'Xem'}>
                              <IconButton
                                component={Link}
                                to={`/request/material/detail/${item.id}`}
                                color="primary"
                              >
                                <IconEye />
                              </IconButton>
                            </Tooltip>

                            <RequireRole allowedRoles={['Regular User', 'Department Head']}>
                              {item.status === 'Pending_Department' && (
                                <>
                                  <Tooltip title={t('Action.Edit') || 'Sửa'}>
                                    <IconButton
                                      component={Link}
                                      to={`/request/material/edit/${item.id}`}
                                      color="success"
                                    >
                                      <IconEdit />
                                    </IconButton>
                                  </Tooltip>
                                  <Tooltip title={t('Action.Delete') || 'Xóa'}>
                                    <IconButton
                                      color="error"
                                      onClick={() => {
                                        setDeleteId(item.id);
                                        setOpenDeleteDialog(true);
                                      }}
                                    >
                                      <IconTrash />
                                    </IconButton>
                                  </Tooltip>
                                </>
                              )}
                            </RequireRole>
                          </Stack>
                        </TableCell>
                      </TableRow>
                    ))
                  )}
                </TableBody>
              </Table>
              <TablePagination
                rowsPerPageOptions={[5, 10, 25]}
                component="div"
                count={filteredRequests.length}
                rowsPerPage={rowsPerPage}
                page={page}
                onPageChange={(e, p) => setPage(p)}
                onRowsPerPageChange={(e) => {
                  setRowsPerPage(parseInt(e.target.value, 10));
                  setPage(0);
                }}
              />
            </>
          )}
        </Box>

        <Dialog open={openDeleteDialog} onClose={handleCloseDeleteDialog}>
          <DialogTitle>{t('Action.Delete') || 'Xóa'}</DialogTitle>
          <DialogContent>
            {t('Message.ConfirmDelete') || 'Bạn có chắc chắn muốn xóa không?'}
          </DialogContent>
          <DialogActions>
            <Button onClick={handleCloseDeleteDialog}>{t('Action.Cancel') || 'Hủy'}</Button>
            <Button color="error" variant="outlined" onClick={handleConfirmDelete}>
              {t('Action.Delete') || 'Xóa'}
            </Button>
          </DialogActions>
        </Dialog>
      </Box>
    </LocalizationProvider>
  );
};

export default MaterialRequestList;