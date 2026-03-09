import React, { useState, useContext, useEffect, useMemo } from 'react';
import {
  Alert,
  Button,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Typography,
  IconButton,
  Box,
  Stack,
  Divider,
  Grid,
  Autocomplete,
  Chip,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Snackbar,
  Radio,
  RadioGroup,
  FormControlLabel,
  FormControl,
  Fade,
  CircularProgress,
} from '@mui/material';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { useNavigate, useParams } from 'react-router';
import { format, isValid } from 'date-fns';
import { IconPlus, IconTrash } from '@tabler/icons-react';
import { vi, enUS } from 'date-fns/locale';
import { useTranslation } from 'react-i18next';

import CustomFormLabel from 'src/components/forms/theme-elements/CustomFormLabel';
import CustomTextField from 'src/components/forms/theme-elements/CustomTextField';

import { MaterialRequestContext } from '../../../../context/MaterialRequestContext';
import { useMasterData, useProducts } from 'src/hooks/useMasterData';
import { stockOutApi } from 'src/api/inventory-transaction/stock-out/stockOutApi';

const REQUEST_TYPES = {
  ISSUE: 1,
  RETURN: 2,
};

const EditMaterialRequestApp = () => {
  const { t, i18n } = useTranslation();
  const router = useNavigate();
  const { id } = useParams();
  const { getMaterialRequestById, updateMaterialRequest, clearError, error } =
    useContext(MaterialRequestContext);

  const { warehouses, units, requesters } = useMasterData();

  const [showAlert, setShowAlert] = useState(false);
  const [openConfirm, setOpenConfirm] = useState(false);
  const [loading, setLoading] = useState(false);
  const [errors, setErrors] = useState({});
  const [referenceOptions, setReferenceOptions] = useState([]);
  const [isRefLoading, setIsRefLoading] = useState(false);

  const [formData, setFormData] = useState({
    id: 0,
    code: '',
    type: REQUEST_TYPES.ISSUE,
    selectedStockOut: null,
    requester: null,
    warehouse: null,
    approver: null,
    purpose: '',
    status: '',
    items: [],
    date: '',
    expectedDate: '',
  });

  const { products } = useProducts('local', formData.warehouse?.id);

  const isReadOnly = !['pending_department', 'pending_warehouse'].includes(
    formData.status?.toLowerCase() || '',
  );

  const referenceProductIds = useMemo(() => {
    if (!formData.selectedStockOut) return [];
    const itemsSource =
      formData.selectedStockOut.stockOutDetails ||
      formData.selectedStockOut.items ||
      formData.selectedStockOut.details ||
      [];

    return itemsSource.map((item) => item.productId);
  }, [formData.selectedStockOut]);

  useEffect(() => {
    if (!id || !warehouses.length || !requesters.length || !products) return;

    const loadRequest = async () => {
      const data = await getMaterialRequestById(Number(id));
      if (!data) return;

      let selectedStockOut = null;
      if (data.type === REQUEST_TYPES.RETURN) {
        selectedStockOut = data.referenceStockOut || (data.referenceId ? { id: data.referenceId, code: data.referenceCode || `Ref #${data.referenceId}` } : null);
      }

      const mappedItems =
        data.items?.map((item) => {
          const productRef = products.find((p) => p.id === item.productId);
          const defaultUnit = units.find((u) => u.id === productRef?.unitId) || null;
          
          let inStockQuantity = productRef?.quantity || 0;
          
          if (data.type === REQUEST_TYPES.RETURN && selectedStockOut) {
            const stockOutItem = (selectedStockOut.stockOutDetails || selectedStockOut.items || selectedStockOut.details || []).find(d => d.productId === item.productId);
            if(stockOutItem) {
               inStockQuantity = stockOutItem.quantity;
            }
          }

          return {
            productId: item.productId,
            itemName: item.productName || '',
            unit: defaultUnit,
            quantity: inStockQuantity,
            requestedQty: item.requestedQuantity || item.quantity || 1,
          };
        }) ?? [];

      setFormData({
        id: data.id,
        code: data.code,
        type: data.type || REQUEST_TYPES.ISSUE,
        selectedStockOut: selectedStockOut,
        requester: requesters.find((r) => r.id === data.requesterId) || null,
        warehouse: warehouses.find((w) => w.id === data.warehouseId) || null,
        approver: requesters.find((r) => r.id === data.approverId) || null,
        purpose: data.purpose || '',
        status: data.status || '',
        items: mappedItems,
        date: data.requestDate,
        expectedDate: data.expectedDate || '',
      });
    };

    loadRequest();
  }, [id, getMaterialRequestById, requesters, warehouses, units, products]);

  useEffect(() => {
    const fetchReferences = async () => {
      if (formData.type !== REQUEST_TYPES.RETURN || !formData.warehouse) {
        setReferenceOptions([]);
        return;
      }

      setIsRefLoading(true);
      try {
        const response = await stockOutApi.getUsage();
        const data = response?.data || response || [];
        setReferenceOptions(Array.isArray(data) ? data : []);
      } catch (err) {
        setReferenceOptions([]);
      } finally {
        setIsRefLoading(false);
      }
    };

    fetchReferences();
  }, [formData.type, formData.warehouse]);

  const handleStockOutSelect = async (stockOut) => {
    if (!stockOut) {
      setFormData((prev) => ({
        ...prev,
        selectedStockOut: null,
        items: [{ productId: null, itemName: '', unit: null, quantity: 0, requestedQty: '' }],
      }));
      return;
    }

    setIsRefLoading(true);
    try {
      const response = await stockOutApi.getById(stockOut.id);
      const fullDetail = response?.data || response;

      const itemsSource =
        fullDetail.stockOutDetails ||
        fullDetail.items ||
        fullDetail.details ||
        [];

      const mappedItems = itemsSource.map((detail) => {
        const productInfo = products?.find((p) => p.id === detail.productId);

        return {
          productId: detail.productId,
          itemName: productInfo?.name || detail.productName || '',
          unit: units?.find((u) => u.id === (productInfo?.unitId || detail.unitId)) || null,
          quantity: detail.quantity || 0,
          requestedQty: '',
        };
      });

      setFormData((prev) => ({
        ...prev,
        selectedStockOut: fullDetail,
        items: mappedItems.length > 0 ? mappedItems : prev.items,
      }));
    } catch (err) {
      console.error(err);
    } finally {
      setIsRefLoading(false);
    }
  };

  const getAvailableProducts = (currentIndex) => {
    let currentProducts = products || [];

    if (formData.type === REQUEST_TYPES.RETURN && formData.selectedStockOut && referenceProductIds.length > 0) {
      currentProducts = currentProducts.filter((p) => referenceProductIds.includes(p.id));
    }

    const selectedIds = formData.items
      .filter((_, i) => i !== currentIndex)
      .map((o) => o.productId)
      .filter(Boolean);

    return currentProducts.filter((p) => !selectedIds.includes(p.id));
  };

  const handleAutocompleteChange = (name, value) => {
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleProductSelect = (index, product) => {
    setFormData((prev) => {
      const items = [...prev.items];
      if (product) {
        const defaultUnit = units.find((u) => u.id === product.unitId) || null;
        
        let inStockQuantity = product.quantity || 0;
        
        if (formData.type === REQUEST_TYPES.RETURN && formData.selectedStockOut) {
             const stockOutItem = (formData.selectedStockOut.stockOutDetails || formData.selectedStockOut.items || formData.selectedStockOut.details || []).find(d => d.productId === product.id);
             if(stockOutItem) {
                 inStockQuantity = stockOutItem.quantity;
             }
        }

        items[index] = {
          ...items[index],
          productId: product.id,
          itemName: product.name,
          unit: defaultUnit,
          quantity: inStockQuantity,
          requestedQty: 1,
        };
      } else {
        items[index] = {
          ...items[index],
          productId: null,
          itemName: '',
          unit: null,
          quantity: 0,
          requestedQty: '',
        };
      }
      return { ...prev, items };
    });
  };

  const handleItemChange = (index, field, value) => {
    setFormData((prev) => {
      const items = [...prev.items];
      items[index] = { ...items[index], [field]: value };
      return { ...prev, items };
    });
  };

  const handleAddItem = () => {
    setFormData((prev) => {
      const items = [
        ...prev.items,
        { productId: null, itemName: '', unit: null, quantity: 0, requestedQty: '' },
      ];
      return { ...prev, items };
    });
  };

  const handleDeleteItem = (index) => {
    setFormData((prev) => {
      const items = prev.items.filter((_, i) => i !== index);
      return { ...prev, items };
    });
  };

  const validateForm = () => {
    const errs = {};
    if (!formData.requester) errs.requester = t('Placeholder.Select') || 'Vui lòng chọn';
    if (!formData.warehouse) errs.warehouse = t('Placeholder.Select') || 'Vui lòng chọn';
    if (!formData.approver) errs.approver = t('Placeholder.Select') || 'Vui lòng chọn';
    if (!formData.purpose.trim()) errs.purpose = t('Validation.Required') || 'Không được để trống';
    if (formData.type === REQUEST_TYPES.RETURN && !formData.selectedStockOut) {
      errs.selectedStockOut = t('Validation.Required');
    }

    if (!formData.items.length) errs.items = t('Message.Error') || 'Cần ít nhất 1 vật tư';
    formData.items.forEach((item, index) => {
      if (!item.productId) errs[`product_${index}`] = t('Placeholder.Select');
      if (!item.requestedQty || Number(item.requestedQty) <= 0) {
        errs[`requestedQty_${index}`] = t('Message.Error');
      }
      if (formData.type === REQUEST_TYPES.RETURN && Number(item.requestedQty) > Number(item.quantity)) {
        errs[`requestedQty_${index}`] = t('Validation.ExceedsAllowedQuantity') || 'Vượt quá SL đã nhận';
      }
    });

    setErrors(errs);
    return Object.keys(errs).length === 0;
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!validateForm()) return;
    setOpenConfirm(true);
  };

  const handleConfirmSave = async () => {
    const payload = {
      id: formData.id,
      type: formData.type,
      referenceId: formData.selectedStockOut?.code,
      requestDate: formData.date,
      requesterId: formData.requester?.id,
      warehouseId: formData.warehouse?.id,
      approverId: formData.approver?.id,
      expectedDate: formData.expectedDate,
      purpose: formData.purpose,
      items: formData.items.map((item) => ({
        productId: item.productId,
        unitId: item.unit?.id,
        quantity: Number(item.requestedQty),
      })),
    };

    setLoading(true);
    const success = await updateMaterialRequest(payload);
    setLoading(false);

    if (success) {
      setOpenConfirm(false);
      setShowAlert(true);
      setTimeout(() => router('/request/material/list'), 800);
    }
  };

  const parsedDate = isValid(new Date(formData.date)) ? new Date(formData.date) : new Date();
  const isVietnamese = i18n.language && i18n.language.startsWith('vi');
  const formattedDate = format(
    parsedDate,
    isVietnamese ? "EEEE, 'ngày' dd 'tháng' MM, yyyy" : 'EEEE, MMMM dd, yyyy',
    { locale: isVietnamese ? vi : enUS },
  );

  const getStatusLabel = (status) => {
    status = status?.toLowerCase() || '';
    if (!status) return '';
    if (status === 'pending_department') return t('Status.Pending_Department') || 'Chờ TP duyệt';
    if (status === 'pending_warehouse') return t('Status.Pending_Warehouse') || 'Chờ Kho duyệt';
    if (status === 'approved') return t('Status.Approved') || 'Đã duyệt';
    if (status === 'rejected') return t('Status.Rejected') || 'Từ chối';
    return status;
  };

  const getStatusColor = (status) => {
    status = status?.toLowerCase() || '';
    if (!status) return 'default';
    if (status.toLowerCase().includes('pending')) return 'warning';
    if (status === 'approved') return 'success';
    if (status === 'rejected') return 'error';
    return 'default';
  };

  return (
    <>
      <Dialog open={openConfirm} onClose={() => setOpenConfirm(false)}>
        <DialogTitle>{t('Action.Update') || 'Cập nhật'}</DialogTitle>
        <DialogContent>
          <Typography>
            {t('Message.ConfirmChange') || 'Bạn có chắc chắn muốn lưu thay đổi?'}
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setOpenConfirm(false)}>{t('Action.Cancel')}</Button>
          <Button
            variant="contained"
            color="primary"
            onClick={handleConfirmSave}
            disabled={loading}
          >
            {loading ? t('Action.Save') : t('Action.Confirm')}
          </Button>
        </DialogActions>
      </Dialog>

      <form onSubmit={handleSubmit}>
        <Box>
          <Stack direction="row" spacing={2} justifyContent="space-between" mb={3}>
            <Typography variant="h5">
              {t('Menu.MaterialRequest') || 'Yêu cầu vật tư'}{' '}
              {formData.code ? `#${formData.code}` : `#${formData.id}`}
            </Typography>
            <Box display="flex" gap={1}>
              <Button
                variant="outlined"
                color="error"
                onClick={() => router('/request/material/list')}
              >
                {t('Action.Cancel')}
              </Button>
              <Button type="submit" variant="contained" disabled={isReadOnly}>
                {t('Action.Save')}
              </Button>
            </Box>
          </Stack>
          <Divider sx={{ mb: 3 }} />

          <Stack direction="row" justifyContent="space-between" alignItems="center" mb={3}>
            <Box mt={1}>
              <Chip
                label={getStatusLabel(formData.status)}
                color={getStatusColor(formData.status)}
                variant="filled"
              />
            </Box>
            <Box textAlign="right">
              <CustomFormLabel>{t('Field.Date') || 'Ngày tạo'}</CustomFormLabel>
              <Typography variant="body1">{formattedDate}</Typography>
            </Box>
          </Stack>

          <Paper variant="outlined" sx={{ p: 2, mb: 4, bgcolor: 'grey.50' }}>
            <Box
              display="flex"
              flexDirection={{ xs: 'column', md: 'row' }}
              justifyContent="space-between"
              alignItems={{ xs: 'start', md: 'center' }}
              gap={2}
            >
              <FormControl component="fieldset">
                <CustomFormLabel sx={{ mt: 0, mb: 0.5 }}>{t('Field.Type')}</CustomFormLabel>
                <RadioGroup row value={formData.type}>
                  <FormControlLabel
                    value={REQUEST_TYPES.ISSUE}
                    control={<Radio disabled />}
                    label={t('RequestType.Issue') || 'Yêu cầu cung cấp'}
                  />
                  <FormControlLabel
                    value={REQUEST_TYPES.RETURN}
                    control={<Radio disabled />}
                    label={t('RequestType.Return') || 'Yêu cầu hoàn trả'}
                  />
                </RadioGroup>
              </FormControl>

              <Fade in={formData.type === REQUEST_TYPES.RETURN} unmountOnExit>
                <Box sx={{ minWidth: { xs: '100%', md: 350 } }}>
                  <CustomFormLabel sx={{ mt: 0, mb: 0.5 }}>
                    {t('Field.Reference') || 'Phiếu tham chiếu'}
                  </CustomFormLabel>
                  <Autocomplete
                    fullWidth
                    size="small"
                    loading={isRefLoading}
                    disabled={isReadOnly}
                    options={referenceOptions}
                    getOptionLabel={(option) => option.code || ''}
                    isOptionEqualToValue={(option, value) => option.id === value?.id}
                    value={formData.selectedStockOut}
                    onChange={(e, v) => handleStockOutSelect(v)}
                    noOptionsText={t('Message.NoData')}
                    renderInput={(params) => (
                      <CustomTextField
                        {...params}
                        placeholder={t('Placeholder.Select')}
                        error={!!errors.selectedStockOut}
                        sx={{ bgcolor: 'white' }}
                        InputProps={{
                          ...params.InputProps,
                          endAdornment: (
                            <React.Fragment>
                              {isRefLoading ? <CircularProgress color="inherit" size={20} /> : null}
                              {params.InputProps.endAdornment}
                            </React.Fragment>
                          ),
                        }}
                      />
                    )}
                  />
                </Box>
              </Fade>
            </Box>
          </Paper>

          <Grid container spacing={3} mb={4}>
            <Grid item size={{ xs: 12, sm: 6 }}>
              <CustomFormLabel>{t('Field.Requester') || 'Người yêu cầu'}</CustomFormLabel>
              <Autocomplete
                fullWidth
                disabled={isReadOnly}
                options={requesters || []}
                getOptionLabel={(o) => o.name}
                value={formData.requester}
                onChange={(e, v) => handleAutocompleteChange('requester', v)}
                renderInput={(p) => (
                  <CustomTextField
                    {...p}
                    error={!!errors.requester}
                    helperText={errors.requester}
                  />
                )}
              />
            </Grid>
            <Grid item size={{ xs: 12, sm: 6 }}>
              <CustomFormLabel>{t('Field.Warehouse') || 'Kho yêu cầu'}</CustomFormLabel>
              <Autocomplete
                fullWidth
                disabled={isReadOnly}
                options={warehouses || []}
                getOptionLabel={(o) => o.name}
                value={formData.warehouse}
                onChange={(e, v) => handleAutocompleteChange('warehouse', v)}
                renderInput={(p) => (
                  <CustomTextField
                    {...p}
                    error={!!errors.warehouse}
                    helperText={errors.warehouse}
                  />
                )}
              />
            </Grid>
            <Grid item size={{ xs: 12, sm: 6 }}>
              <CustomFormLabel>{t('Field.Approver') || 'Người phê duyệt'}</CustomFormLabel>
              <Autocomplete
                fullWidth
                disabled={isReadOnly}
                options={requesters || []}
                getOptionLabel={(o) => o.name}
                value={formData.approver}
                onChange={(e, v) => handleAutocompleteChange('approver', v)}
                renderInput={(p) => (
                  <CustomTextField {...p} error={!!errors.approver} helperText={errors.approver} />
                )}
              />
            </Grid>
            <Grid item size={{ xs: 12, sm: 6 }}>
              <CustomFormLabel>{t('Field.ExpectedDate') || 'Ngày dự kiến'}</CustomFormLabel>
              <LocalizationProvider dateAdapter={AdapterDateFns}>
                <DatePicker
                  format="dd/MM/yyyy"
                  value={formData.expectedDate ? new Date(formData.expectedDate) : null}
                  onChange={(newValue) => {
                    if (!newValue) return;
                    setFormData((prev) => ({
                      ...prev,
                      expectedDate: format(newValue, 'yyyy-MM-dd'),
                    }));
                  }}
                  disabled={isReadOnly}
                  enableAccessibleFieldDOMStructure={false}
                  slots={{ textField: CustomTextField }}
                  slotProps={{
                    textField: {
                      fullWidth: true,
                      error: !!errors.expectedDate,
                      helperText: errors.expectedDate,
                    },
                  }}
                />
              </LocalizationProvider>
            </Grid>
            <Grid item size={{ xs: 12 }}>
              <CustomFormLabel>{t('Field.Purpose') || 'Mục đích/Ghi chú'}</CustomFormLabel>
              <CustomTextField
                fullWidth
                multiline
                rows={2}
                disabled={isReadOnly}
                value={formData.purpose}
                onChange={(e) => setFormData({ ...formData, purpose: e.target.value })}
                error={!!errors.purpose}
                helperText={errors.purpose}
              />
            </Grid>
          </Grid>

          <Stack direction="row" justifyContent="space-between" mb={2}>
            <Typography variant="h6">{t('Field.Items') || 'Danh sách vật tư'}</Typography>
            {!isReadOnly && (
              <Button onClick={handleAddItem} startIcon={<IconPlus />}>
                {t('Action.Add')}
              </Button>
            )}
          </Stack>

          <Paper variant="outlined">
            <TableContainer>
              <Table>
                <TableHead>
                  <TableRow>
                    <TableCell width="35%">{t('Entity.Product') || 'Sản phẩm'}</TableCell>
                    <TableCell width="20%">{t('Menu.Unit') || 'Đơn vị'}</TableCell>
                    <TableCell width="15%">
                      {formData.type === REQUEST_TYPES.RETURN 
                        ? (t('Field.ReceivedQty') || 'SL đã nhận') 
                        : (t('Field.InStock') || 'Tồn kho')}
                    </TableCell>
                    <TableCell width="20%">
                      {t('Field.RequestedQty') || 'S.Lượng Yêu cầu'}
                    </TableCell>
                    <TableCell width="10%">{t('Field.Action')}</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {formData.items.map((item, index) => (
                    <TableRow key={index}>
                      <TableCell sx={{ verticalAlign: 'top' }}>
                        <Autocomplete
                          disabled={isReadOnly}
                          options={getAvailableProducts(index)}
                          getOptionLabel={(o) => o.name}
                          isOptionEqualToValue={(o, v) => o.id === v.id}
                          value={products?.find((p) => p.id === item.productId) || null}
                          onChange={(e, v) => handleProductSelect(index, v)}
                          renderInput={(p) => (
                            <CustomTextField
                              {...p}
                              error={!!errors[`product_${index}`]}
                              helperText={errors[`product_${index}`]}
                            />
                          )}
                        />
                      </TableCell>
                      <TableCell sx={{ verticalAlign: 'top' }}>
                        <CustomTextField
                          value={item.unit ? item.unit.name : ''}
                          disabled
                          sx={{ bgcolor: 'grey.100' }}
                        />
                      </TableCell>
                      <TableCell sx={{ verticalAlign: 'top' }}>
                        <CustomTextField
                          type="number"
                          disabled
                          value={item.quantity}
                          sx={{ bgcolor: 'grey.100' }}
                        />
                      </TableCell>
                      <TableCell sx={{ verticalAlign: 'top' }}>
                        <CustomTextField
                          type="number"
                          disabled={isReadOnly}
                          value={item.requestedQty}
                          onChange={(e) => handleItemChange(index, 'requestedQty', e.target.value)}
                          error={!!errors[`requestedQty_${index}`]}
                          helperText={errors[`requestedQty_${index}`]}
                        />
                      </TableCell>
                      <TableCell>
                        {!isReadOnly && (
                          <IconButton onClick={() => handleDeleteItem(index)} color="error">
                            <IconTrash width={22} />
                          </IconButton>
                        )}
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          </Paper>
        </Box>
        <Snackbar
          open={!!error}
          autoHideDuration={4000}
          onClose={() => clearError()}
          anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
        >
          <Alert severity="error" onClose={() => clearError()}>
            {error?.response?.data?.message || t('Message.Error')}
          </Alert>
        </Snackbar>
      </form>
    </>
  );
};

export default EditMaterialRequestApp;