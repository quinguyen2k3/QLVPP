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
import { useNavigate } from 'react-router';
import { format } from 'date-fns';
import { IconSquareRoundedPlus, IconTrash } from '@tabler/icons-react';
import CustomFormLabel from 'src/components/forms/theme-elements/CustomFormLabel';
import CustomTextField from 'src/components/forms/theme-elements/CustomTextField';
import { MaterialRequestContext } from '../../../../context/MaterialRequestContext';
import { useMasterData, useProducts } from 'src/hooks/useMasterData';
import { useTranslation } from 'react-i18next';
import { stockOutApi } from 'src/api/inventory-transaction/stock-out/stockOutApi';

const REQUEST_TYPES = {
  ISSUE: 1,
  RETURN: 2,
};

const CreateMaterialRequest = ({ type = REQUEST_TYPES.ISSUE }) => {
  const { t } = useTranslation();
  const [showAlert, setShowAlert] = useState(false);
  const { addMaterialRequest } = useContext(MaterialRequestContext);
  const router = useNavigate();

  const { warehouses, units, requesters } = useMasterData();

  const [errors, setErrors] = useState({});
  const [referenceOptions, setReferenceOptions] = useState([]);
  const [isRefLoading, setIsRefLoading] = useState(false);

  const [formData, setFormData] = useState({
    type: type,
    selectedStockOut: null,
    requester: null,
    warehouse: null,
    approver: null,
    purpose: '',
    expectedDate: new Date().toISOString().split('T')[0],
    items: [{ productId: null, itemName: '', unit: null, quantity: 0, requestedQty: '' }],
  });

  const { products } = useProducts('local', formData.warehouse?.id);

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
    if (type !== formData.type) {
      setFormData((prev) => ({
        ...prev,
        type: type,
        selectedStockOut: null,
        items: [{ productId: null, itemName: '', unit: null, quantity: 0, requestedQty: '' }],
      }));
      setErrors({});
    }
  }, [type]);

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
      } catch (error) {
        setReferenceOptions([]);
      } finally {
        setIsRefLoading(false);
      }
    };

    fetchReferences();
  }, [formData.type, formData.warehouse]);

  const handleTypeChange = (event) => {
    const newType = parseInt(event.target.value);
    setFormData((prev) => ({
      ...prev,
      type: newType,
      selectedStockOut: null,
      items: [{ productId: null, itemName: '', unit: null, quantity: 0, requestedQty: '' }],
    }));
    setErrors({});
  };

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
    } catch (error) {
      console.error(error);
    } finally {
      setIsRefLoading(false);
    }
  };

  const getAvailableProducts = (currentIndex) => {
    let currentProducts = products || [];

    if (formData.selectedStockOut && referenceProductIds.length > 0) {
      currentProducts = currentProducts.filter((p) => referenceProductIds.includes(p.id));
    }

    const selectedProductIds = formData.items
      .filter((_, i) => i !== currentIndex)
      .map((item) => item.productId)
      .filter(Boolean);

    return currentProducts.filter((product) => !selectedProductIds.includes(product.id));
  };

  const handleProductSelect = (index, product) => {
    setFormData((prevData) => {
      const updatedItems = [...prevData.items];
      if (product) {
        updatedItems[index] = {
          ...updatedItems[index],
          productId: product.id,
          itemName: product.name,
          unit: (units || []).find((u) => u.id === product.unitId) || null,
          quantity: product.quantity || 0,
          requestedQty: 1,
        };
      } else {
        updatedItems[index] = {
          ...updatedItems[index],
          productId: null,
          itemName: '',
          unit: null,
          quantity: 0,
          requestedQty: '',
        };
      }
      return { ...prevData, items: updatedItems };
    });
  };

  const handleUnitChange = (index, unit) => {
    setFormData((prevData) => {
      const updatedItems = [...prevData.items];
      updatedItems[index] = { ...updatedItems[index], unit: unit };
      return { ...prevData, items: updatedItems };
    });
  };

  const handleItemChange = (index, field, value) => {
    setFormData((prevData) => {
      const updatedItems = [...prevData.items];
      updatedItems[index] = { ...updatedItems[index], [field]: value };
      return { ...prevData, items: updatedItems };
    });
  };

  const handleAddItem = () => {
    setFormData((prevData) => ({
      ...prevData,
      items: [
        ...prevData.items,
        { productId: null, itemName: '', unit: null, quantity: 0, requestedQty: '' },
      ],
    }));
  };

  const handleDeleteItem = (index) => {
    setFormData((prevData) => {
      const updatedItems = prevData.items.filter((_, i) => i !== index);
      return { ...prevData, items: updatedItems };
    });
  };

  const validateForm = () => {
    const newErrors = {};
    if (!formData.requester) newErrors.requester = t('Validation.Required');
    if (!formData.warehouse) newErrors.warehouse = t('Validation.Required');
    if (!formData.approver) newErrors.approver = t('Validation.Required');
    if (!formData.purpose.trim()) newErrors.purpose = t('Validation.Required');
    if (formData.type === REQUEST_TYPES.RETURN && !formData.selectedStockOut) {
      newErrors.selectedStockOut = t('Validation.Required');
    }
    if (!formData.items.length) newErrors.items = t('Validation.AtLeastOneItem');

    formData.items.forEach((item, index) => {
      if (!item.productId) newErrors[`product_${index}`] = t('Validation.Required');
      if (!item.requestedQty || Number(item.requestedQty) <= 0) {
        newErrors[`requestedQty_${index}`] = t('Validation.InvalidQuantity');
      }
      if (formData.type === REQUEST_TYPES.RETURN && Number(item.requestedQty) > Number(item.quantity)) {
        newErrors[`requestedQty_${index}`] = t('Validation.ExceedsAllowedQuantity') || 'Vượt quá SL đã nhận';
      }
    });

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validateForm()) return;

    const payload = {
      type: formData.type,
      referenceId: formData.selectedStockOut?.code,
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

    try {
      if (addMaterialRequest) {
        await addMaterialRequest(payload);
      }
      setShowAlert(true);
      setTimeout(() => router('/request/material/list'), 1000);
    } catch (error) {
      console.error(error);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <Box>
        <Stack direction="row" spacing={2} justifyContent="space-between" mb={3}>
          <Typography variant="h5">
            {t('Menu.MaterialRequest') || 'Yêu cầu cung cấp vật tư'}
          </Typography>
          <Box display="flex" gap={1}>
            <Button
              variant="outlined"
              color="error"
              onClick={() => router('/request/material/list')}
            >
              {t('Action.Cancel')}
            </Button>
            <Button type="submit" variant="contained" color="primary">
              {t('Action.Create')}
            </Button>
          </Box>
        </Stack>
        <Divider sx={{ mb: 3 }} />

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
              <RadioGroup row value={formData.type} onChange={handleTypeChange}>
                <FormControlLabel
                  value={REQUEST_TYPES.ISSUE}
                  control={<Radio />}
                  label={t('RequestType.Issue') || 'Yêu cầu cung cấp'}
                />
                <FormControlLabel
                  value={REQUEST_TYPES.RETURN}
                  control={<Radio />}
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

        <Paper variant="outlined" sx={{ p: 3, mb: 4, bgcolor: 'grey.50' }}>
          <Grid container spacing={3}>
            <Grid item size={{ xs: 12, sm: 6 }}>
              <CustomFormLabel>{t('Field.Requester') || 'Người tạo'}</CustomFormLabel>
              <Autocomplete
                fullWidth
                options={requesters || []}
                groupBy={(option) => option.departmentName}
                getOptionLabel={(o) => o.name}
                value={formData.requester}
                onChange={(e, v) => setFormData({ ...formData, requester: v })}
                renderInput={(p) => <CustomTextField {...p} error={!!errors.requester} />}
              />
            </Grid>
            <Grid item size={{ xs: 12, sm: 6 }}>
              <CustomFormLabel>{t('Field.Warehouse') || 'Kho yêu cầu'}</CustomFormLabel>
              <Autocomplete
                fullWidth
                options={warehouses || []}
                getOptionLabel={(o) => o.name}
                value={formData.warehouse}
                onChange={(e, v) => setFormData({ ...formData, warehouse: v })}
                renderInput={(p) => <CustomTextField {...p} error={!!errors.warehouse} />}
              />
            </Grid>
            <Grid item size={{ xs: 12, sm: 6 }}>
              <CustomFormLabel>{t('Field.Approver') || 'Người phê duyệt'}</CustomFormLabel>
              <Autocomplete
                fullWidth
                options={requesters || []}
                groupBy={(option) => option.departmentName}
                getOptionLabel={(o) => o.name}
                value={formData.approver}
                onChange={(e, v) => setFormData({ ...formData, approver: v })}
                renderInput={(p) => <CustomTextField {...p} error={!!errors.approver} />}
              />
            </Grid>
            <Grid item size={{ xs: 12, sm: 6 }}>
              <CustomFormLabel>{t('Field.ExpectedDate') || 'Ngày dự kiến'}</CustomFormLabel>
              <LocalizationProvider dateAdapter={AdapterDateFns}>
                <DatePicker
                  format="dd/MM/yyyy"
                  value={formData.expectedDate ? new Date(formData.expectedDate) : null}
                  enableAccessibleFieldDOMStructure={false}
                  onChange={(newValue) =>
                    setFormData((prev) => ({ ...prev, expectedDate: format(newValue, 'yyyy-MM-dd') }))
                  }
                  slots={{ textField: CustomTextField }}
                  slotProps={{ textField: { fullWidth: true } }}
                />
              </LocalizationProvider>
            </Grid>
            <Grid item size={{ xs: 12 }}>
              <CustomFormLabel>{t('Field.Purpose') || 'Mục đích/Ghi chú'}</CustomFormLabel>
              <CustomTextField
                fullWidth
                multiline
                rows={2}
                value={formData.purpose}
                onChange={(e) => setFormData({ ...formData, purpose: e.target.value })}
                error={!!errors.purpose}
              />
            </Grid>
          </Grid>
        </Paper>

        <Stack direction="row" justifyContent="space-between" mb={2}>
          <Typography variant="h6">{t('Field.Items') || 'Danh sách vật tư'}</Typography>
          <Button
            onClick={handleAddItem}
            variant="contained"
            startIcon={<IconSquareRoundedPlus width={18} />}
          >
            {t('Action.Add')}
          </Button>
        </Stack>

        <Paper variant="outlined">
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell width="35%">{t('Menu.Product') || 'Tên sản phẩm'}</TableCell>
                  <TableCell width="20%">{t('Menu.Unit') || 'Đơn vị'}</TableCell>
                  <TableCell width="15%">
                    {formData.type === REQUEST_TYPES.RETURN 
                      ? (t('Field.ReceivedQty') || 'SL đã nhận') 
                      : (t('Field.InStock') || 'SL trong kho')}
                  </TableCell>
                  <TableCell width="20%">{t('Field.RequestedQty') || 'Số lượng yêu cầu'}</TableCell>
                  <TableCell width="10%">{t('Field.Action') || 'Hành động'}</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {formData.items.map((item, index) => (
                  <TableRow key={index}>
                    <TableCell sx={{ verticalAlign: 'top' }}>
                      <Autocomplete
                        options={getAvailableProducts(index)}
                        getOptionLabel={(o) => o.name}
                        value={products?.find((p) => p.id === item.productId) || null}
                        onChange={(e, v) => handleProductSelect(index, v)}
                        renderInput={(p) => (
                          <CustomTextField {...p} error={!!errors[`product_${index}`]} />
                        )}
                      />
                    </TableCell>
                    <TableCell sx={{ verticalAlign: 'top' }}>
                      <Autocomplete
                        options={units || []}
                        getOptionLabel={(o) => o.name}
                        value={item.unit}
                        isOptionEqualToValue={(option, value) => option.id === value?.id}
                        onChange={(e, v) => handleUnitChange(index, v)}
                        renderInput={(p) => (
                          <CustomTextField {...p} error={!!errors[`unit_${index}`]} />
                        )}
                      />
                    </TableCell>
                    <TableCell sx={{ verticalAlign: 'top' }}>
                      <CustomTextField
                        type="number"
                        value={item.quantity}
                        disabled
                        sx={{ bgcolor: 'grey.100' }}
                      />
                    </TableCell>
                    <TableCell sx={{ verticalAlign: 'top' }}>
                      <CustomTextField
                        type="number"
                        value={item.requestedQty}
                        onChange={(e) => handleItemChange(index, 'requestedQty', e.target.value)}
                        error={!!errors[`requestedQty_${index}`]}
                        helperText={errors[`requestedQty_${index}`]}
                      />
                    </TableCell>
                    <TableCell>
                      <IconButton onClick={() => handleDeleteItem(index)} color="error">
                        <IconTrash width={22} />
                      </IconButton>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        </Paper>

        <Snackbar
          open={showAlert}
          autoHideDuration={3000}
          onClose={() => setShowAlert(false)}
          anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
        >
          <Alert severity="success">{t('Message.Success') || 'Thành công'}</Alert>
        </Snackbar>
      </Box>
    </form>
  );
};

export default CreateMaterialRequest;