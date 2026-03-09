import React, { useContext, useEffect, useState, useMemo } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { vi, enUS } from 'date-fns/locale';
import { useTranslation } from 'react-i18next';
import {
  Typography,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Box,
  Stack,
  Chip,
  Divider,
  Grid,
  CircularProgress,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Autocomplete,
} from '@mui/material';
import CancelPresentationIcon from '@mui/icons-material/CancelPresentation';
import AssignmentTurnedInIcon from '@mui/icons-material/AssignmentTurnedIn';
import ReplyIcon from '@mui/icons-material/Reply';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import EditIcon from '@mui/icons-material/Edit';

import { format, isValid, parseISO } from 'date-fns';

import CustomFormLabel from 'src/components/forms/theme-elements/CustomFormLabel';
import CustomTextField from 'src/components/forms/theme-elements/CustomTextField';

import { MaterialRequestContext } from 'src/context/MaterialRequestContext';
import { useMasterData } from 'src/hooks/useMasterData';
import Logo from 'src/layouts/full/shared/logo/Logo';

const MaterialRequestDetail = () => {
  const { t, i18n } = useTranslation();
  const { id } = useParams();
  const navigate = useNavigate();

  const {
    getMaterialRequestById,
    approveMaterialRequest,
    cancelMaterialRequest,
    delegateMaterialRequest,
  } = useContext(MaterialRequestContext);

  const { warehouses, requesters, products, units } = useMasterData();

  const [request, setRequest] = useState(null);
  const [loading, setLoading] = useState(true);

  const [actionDialog, setActionDialog] = useState({ open: false, type: '' });
  const [actionComment, setActionComment] = useState('');
  const [selectedDelegate, setSelectedDelegate] = useState(null);

  const maps = useMemo(() => {
    const createMap = (arr) =>
      (arr || []).reduce((acc, item) => ({ ...acc, [item.id]: item.name }), {});

    const createProductUnitMap = (arr) =>
      (arr || []).reduce((acc, item) => ({ ...acc, [item.id]: item.unitId }), {});

    return {
      warehouses: createMap(warehouses),
      requesters: createMap(requesters),
      products: createMap(products),
      units: createMap(units),
      productUnits: createProductUnitMap(products),
    };
  }, [warehouses, requesters, products, units]);

  useEffect(() => {
    const fetchDetail = async () => {
      setLoading(true);
      try {
        const data = await getMaterialRequestById(Number(id));
        if (data) {
          setRequest(data);
        }
      } catch (error) {
        console.error(error);
      } finally {
        setLoading(false);
      }
    };
    fetchDetail();
  }, [id, getMaterialRequestById]);

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" alignItems="center" height="50vh">
        <CircularProgress />
      </Box>
    );
  }

  if (!request) {
    return <Typography p={3}>{t('Message.NoData')}</Typography>;
  }

  const isVietnamese = i18n.language === 'vi';
  const dateLocale = isVietnamese ? vi : enUS;
  const dateFormatStr = isVietnamese ? "EEEE, 'ngày' dd 'tháng' MM, yyyy" : 'EEEE, MMMM dd, yyyy';

  const requestDate =
    request.requestDate && isValid(parseISO(request.requestDate))
      ? format(parseISO(request.requestDate), dateFormatStr, { locale: dateLocale })
      : '';

  const getStatusColor = (status) => {
    status = status?.toLowerCase() || '';
    if (!status) return 'default';
    if (status.includes('pending')) return 'warning';
    if (status === 'approved') return 'success';
    if (status === 'rejected' || status === 'cancelled') return 'error';
    return 'default';
  };

  const getStatusLabel = (status) => {
    status = status?.toLowerCase() || '';
    if (!status) return '';
    if (status === 'pending_department') return t('Status.Pending_Department') || 'Chờ TP duyệt';
    if (status === 'pending_warehouse') return t('Status.Pending_Warehouse') || 'Chờ Kho duyệt';
    if (status === 'approved') return t('Status.Approved') || 'Đã duyệt';
    if (status === 'rejected') return t('Status.Rejected') || 'Từ chối';
    return status;
  };

  const isPending = request.status?.toLowerCase().includes('pending');

  const handleOpenActionDialog = (type) => {
    setActionDialog({ open: true, type });
    setActionComment('');
    setSelectedDelegate(null);
  };

  const handleCloseActionDialog = () => {
    setActionDialog({ open: false, type: '' });
  };

  const handleConfirmAction = async () => {
    try {
      if (actionDialog.type === 'approve') {
        await approveMaterialRequest(request.id, actionComment);
      } else if (actionDialog.type === 'reject') {
        await cancelMaterialRequest(request.id, actionComment);
      } else if (actionDialog.type === 'delegate') {
        if (delegateMaterialRequest) {
          await delegateMaterialRequest(request.id, selectedDelegate?.id, actionComment);
        }
      }
      handleCloseActionDialog();
      navigate('/request/material/list');
    } catch (error) {
      console.error(error);
    }
  };

  const getDialogTitle = () => {
    if (actionDialog.type === 'approve') return t('Action.Approve') || 'Phê duyệt';
    if (actionDialog.type === 'reject') return t('Action.Reject') || 'Từ chối';
    if (actionDialog.type === 'delegate') return t('Action.Delegate') || 'Ủy quyền';
    return '';
  };

  const isConfirmDisabled = actionDialog.type === 'delegate' && !selectedDelegate;

  return (
    <>
      {isPending && (
        <Box
          display="flex"
          justifyContent="flex-start"
          gap={1}
          p={2}
          bgcolor="primary.light"
          borderRadius={1}
          mb={3}
        >
          <Button
            variant="contained"
            color="success"
            startIcon={<AssignmentTurnedInIcon />}
            onClick={() => handleOpenActionDialog('approve')}
          >
            {t('Action.Approve') || 'Phê duyệt'}
          </Button>

          <Button
            variant="outlined"
            color="error"
            startIcon={<CancelPresentationIcon />}
            onClick={() => handleOpenActionDialog('reject')}
          >
            {t('Action.Reject') || 'Từ chối'}
          </Button>

          <Button
            variant="contained"
            color="info"
            startIcon={<ReplyIcon />}
            onClick={() => handleOpenActionDialog('delegate')}
          >
            {t('Action.Delegate') || 'Ủy quyền'}
          </Button>
        </Box>
      )}

      <Stack
        direction={{ xs: 'column', sm: 'row' }}
        alignItems="center"
        justifyContent="space-between"
        mb={3}
      >
        <Box textAlign={{ xs: 'center', sm: 'left' }}>
          <Stack direction="row" alignItems="center" gap={1} mb={0.5}>
            <Typography variant="h4" fontWeight={700}>
              {request.code || `MR-${request.id}`}
            </Typography>
          </Stack>
          <Typography variant="body2" color="text.secondary">
            {requestDate}
          </Typography>
        </Box>

        <Box my={{ xs: 2, sm: 0 }}>
          <Logo />
        </Box>

        <Box textAlign="right">
          <Chip
            label={getStatusLabel(request.status)}
            color={getStatusColor(request.status)}
            sx={{ fontWeight: 'bold', px: 1 }}
          />
        </Box>
      </Stack>

      <Divider />

      <Grid container spacing={3} mt={1} mb={4}>
        <Grid item size={{ xs: 12, sm: 4 }}>
          <Paper variant="outlined" sx={{ height: '100%' }}>
            <Box p={3}>
              <Typography variant="h6" mb={1} color="text.secondary" fontSize="0.875rem">
                {t('Field.Warehouse') || 'Kho yêu cầu'}
              </Typography>
              <Typography variant="subtitle1" fontWeight={600}>
                {maps.warehouses[request.warehouseId] || 'Unknown Warehouse'}
              </Typography>
            </Box>
          </Paper>
        </Grid>

        <Grid item size={{ xs: 12, sm: 4 }}>
          <Paper variant="outlined" sx={{ height: '100%' }}>
            <Box p={3}>
              <Typography variant="h6" mb={1} color="text.secondary" fontSize="0.875rem">
                {t('Entity.Requester') || 'Người yêu cầu'}
              </Typography>
              <Typography variant="subtitle1" fontWeight={600}>
                {maps.requesters[request.requesterId] || 'Unknown Requester'}
              </Typography>
            </Box>
          </Paper>
        </Grid>

        <Grid item size={{ xs: 12, sm: 4 }}>
          <Paper variant="outlined" sx={{ height: '100%' }}>
            <Box p={3}>
              <Typography variant="h6" mb={1} color="text.secondary" fontSize="0.875rem">
                {t('Field.Approver') || 'Người phê duyệt'}
              </Typography>
              <Typography variant="subtitle1" fontWeight={600}>
                {maps.requesters[request.approverId] || 'Unknown Approver'}
              </Typography>
            </Box>
          </Paper>
        </Grid>
      </Grid>

      <Paper variant="outlined">
        <TableContainer>
          <Table>
            <TableHead>
              <TableRow sx={{ bgcolor: 'grey.100' }}>
                <TableCell width="50%">
                  <Typography variant="subtitle2" fontWeight={600}>
                    {t('Entity.Product') || 'Sản phẩm'}
                  </Typography>
                </TableCell>
                <TableCell width="25%">
                  <Typography variant="subtitle2" fontWeight={600}>
                    {t('Menu.Unit') || 'Đơn vị'}
                  </Typography>
                </TableCell>
                <TableCell width="25%">
                  <Typography variant="subtitle2" fontWeight={600}>
                    {t('Field.RequestedQty') || 'S.Lượng yêu cầu'}
                  </Typography>
                </TableCell>
              </TableRow>
            </TableHead>

            <TableBody>
              {(request.items ?? []).map((item, index) => {
                const quantity = item.quantity || item.requestedQuantity || 0;
                const productUnitId = maps.productUnits[item.productId];
                const unitName = maps.units[productUnitId] || '-';

                return (
                  <TableRow key={index} hover>
                    <TableCell>
                      <Box>
                        <Typography variant="subtitle2" fontWeight={600}>
                          {maps.products[item.productId] ||
                            item.productName ||
                            `Product #${item.productId}`}
                        </Typography>
                      </Box>
                    </TableCell>
                    <TableCell>{unitName}</TableCell>
                    <TableCell>
                      <Chip label={quantity} size="small" variant="outlined" />
                    </TableCell>
                  </TableRow>
                );
              })}
            </TableBody>
          </Table>
        </TableContainer>
      </Paper>

      {request.purpose && (
        <Paper variant="outlined" sx={{ mt: 3, bgcolor: 'grey.50' }}>
          <Box p={3}>
            <Typography variant="h6" mb={1} color="text.secondary" fontSize="0.875rem">
              {t('Field.Purpose') || 'Mục đích/Ghi chú'}
            </Typography>
            <Typography variant="body1">{request.purpose}</Typography>
          </Box>
        </Paper>
      )}

      <Box display="flex" justifyContent="flex-end" gap={2} mt={3}>
        {request.status?.toLowerCase().includes('pending') && (
          <Button
            variant="contained"
            color="secondary"
            startIcon={<EditIcon />}
            component={Link}
            to={`/request/material/edit/${request.id}`}
          >
            {t('Action.Edit')}
          </Button>
        )}

        <Button
          variant="outlined"
          color="inherit"
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate('/request/material/list')}
        >
          {t('Action.Back')}
        </Button>
      </Box>

      <Dialog open={actionDialog.open} onClose={handleCloseActionDialog} fullWidth maxWidth="sm">
        <DialogTitle>{getDialogTitle()}</DialogTitle>
        <DialogContent>
          {actionDialog.type === 'delegate' && (
            <Box mt={1} mb={2}>
              <CustomFormLabel>{t('Field.DelegateTo') || 'Người được ủy quyền'}</CustomFormLabel>
              <Autocomplete
                options={requesters || []}
                getOptionLabel={(o) => o.name}
                value={selectedDelegate}
                onChange={(e, v) => setSelectedDelegate(v)}
                renderInput={(params) => (
                  <CustomTextField
                    {...params}
                    placeholder={t('Placeholder.Select') || 'Chọn nhân viên...'}
                  />
                )}
              />
            </Box>
          )}

          <Box mt={actionDialog.type === 'delegate' ? 0 : 1}>
            <CustomFormLabel>{t('Field.Note') || 'Ghi chú / Lý do'}</CustomFormLabel>
            <CustomTextField
              fullWidth
              multiline
              rows={3}
              value={actionComment}
              onChange={(e) => setActionComment(e.target.value)}
              placeholder={t('Placeholder.Note') || 'Nhập nội dung...'}
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseActionDialog} color="inherit">
            {t('Action.Cancel') || 'Hủy'}
          </Button>
          <Button
            onClick={handleConfirmAction}
            variant="contained"
            color={actionDialog.type === 'reject' ? 'error' : 'primary'}
            disabled={isConfirmDisabled}
          >
            {t('Action.Confirm') || 'Xác nhận'}
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
};

export default MaterialRequestDetail;