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
  Radio,
  RadioGroup,
  FormControlLabel,
  FormControl,
  Snackbar,
  Fade,
  CircularProgress,
} from '@mui/material';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { AdapterDateFns } from '@mui/x-date-pickers/AdapterDateFns';
import { useNavigate, useLocation } from 'react-router';
import { format } from 'date-fns';
import { IconSquareRoundedPlus, IconTrash } from '@tabler/icons-react';
import CustomFormLabel from 'src/components/forms/theme-elements/CustomFormLabel';
import CustomTextField from 'src/components/forms/theme-elements/CustomTextField';
import { StockInContext } from '../../../../context/StockInContext';
import { useMasterData } from 'src/hooks/useMasterData';
import { useTranslation } from 'react-i18next';
import { stockOutApi } from 'src/api/inventory-transaction/stock-out/stockOutApi';
import { stockTakeApi } from 'src/api/inventory-transaction/stock-take/stockTakeApi';

const STOCK_IN_TYPES = {
  PURCHASE: 1,
  TRANSFER: 2,
  RETURN: 3,
  ADJUSTMENT: 4,
};

const CreateStockInApp = ({ type = STOCK_IN_TYPES.PURCHASE }) => {
  const { t } = useTranslation();
  const location = useLocation();
  const [showAlert, setShowAlert] = useState(false);
  const { addStockIn } = useContext(StockInContext);
  const router = useNavigate();

  const { warehouses, suppliers, products, units, requesters, departments } = useMasterData();

  const [errors, setErrors] = useState({});
  const [referenceOptions, setReferenceOptions] = useState([]);
  const [isRefLoading, setIsRefLoading] = useState(false);

  const [formData, setFormData] = useState({
    id: 0,
    type: type,
    requester: null,
    warehouse: null,
    supplier: null,
    fromWarehouse: null,
    fromDepartment: null,
    selectedStockOut: null,
    note: '',
    status: 'Pending',
    orders: [
      { productId: null, itemName: '', unit: null, unitPrice: '', units: '', unitTotalPrice: 0 },
    ],
    grandTotal: 0,
    subtotal: 0,
    date: new Date().toISOString().split('T')[0],
  });

  const referenceProductIds = useMemo(() => {
    if (!formData.selectedStockOut) return [];
    const itemsSource =
      formData.selectedStockOut.stockOutDetails ||
      formData.selectedStockOut.stockTakeDetails ||
      formData.selectedStockOut.items ||
      formData.selectedStockOut.details ||
      [];

    return itemsSource
      .filter((item) => {
        if (formData.type === STOCK_IN_TYPES.ADJUSTMENT) {
          const diff =
            item.difference !== undefined ? item.difference : item.actualQty - item.sysQty;
          return diff < 0;
        }
        return true;
      })
      .map((item) => item.productId);
  }, [formData.selectedStockOut, formData.type]);

  useEffect(() => {
    const fetchReferences = async () => {
      const validTypes = [
        STOCK_IN_TYPES.TRANSFER,
        STOCK_IN_TYPES.RETURN,
        STOCK_IN_TYPES.ADJUSTMENT,
      ];

      if (!validTypes.includes(formData.type)) {
        setReferenceOptions([]);
        return;
      }

      if (formData.type !== STOCK_IN_TYPES.ADJUSTMENT && !formData.warehouse) {
        setReferenceOptions([]);
        return;
      }

      setIsRefLoading(true);
      try {
        let response = null;
        if (formData.type === STOCK_IN_TYPES.TRANSFER) {
          response = await stockOutApi.getTransferReceived();
        } else if (formData.type === STOCK_IN_TYPES.RETURN) {
          response = await stockOutApi.getUsage();
        } else if (formData.type === STOCK_IN_TYPES.ADJUSTMENT) {
          response = await stockTakeApi.getAll();
        }

        const data = response?.data || response || [];
        const options = Array.isArray(data) ? data : [];
        setReferenceOptions(options);

        const queryParams = new URLSearchParams(location.search);
        const refId = queryParams.get('refId');
        if (refId && options.length > 0) {
          const autoSelect = options.find((opt) => opt.id === Number(refId) || opt.code === refId);
          if (autoSelect) {
            handleStockOutSelect(autoSelect);
          }
        }
      } catch (error) {
        setReferenceOptions([]);
      } finally {
        setIsRefLoading(false);
      }
    };

    fetchReferences();
  }, [formData.type, formData.warehouse, location.search]);

  useEffect(() => {
    if (type !== formData.type) {
      setFormData((prev) => ({
        ...prev,
        type: type,
        supplier: null,
        fromWarehouse: null,
        fromDepartment: null,
        selectedStockOut: null,
        note: '',
        orders: [
          {
            productId: null,
            itemName: '',
            unit: null,
            unitPrice: '',
            units: '',
            unitTotalPrice: 0,
          },
        ],
        grandTotal: 0,
      }));
    }
  }, [type]);

  const calculateTotals = (orders) => {
    if (formData.type !== STOCK_IN_TYPES.PURCHASE) return { subtotal: 0, grandTotal: 0 };
    let subtotal = 0;
    orders.forEach((order) => {
      const unitPrice = parseFloat(order.unitPrice) || 0;
      const unitsQty = parseInt(order.units) || 0;
      const totalCost = unitPrice * unitsQty;
      subtotal += totalCost;
      order.unitTotalPrice = totalCost;
    });
    return { subtotal, grandTotal: subtotal };
  };

  const handleTypeChange = (event) => {
    const newType = parseInt(event.target.value);
    setFormData((prev) => ({
      ...prev,
      type: newType,
      supplier: null,
      fromWarehouse: null,
      fromDepartment: null,
      selectedStockOut: null,
      note: '',
      orders: [
        { productId: null, itemName: '', unit: null, unitPrice: '', units: '', unitTotalPrice: 0 },
      ],
      grandTotal: 0,
    }));
    setErrors({});
  };

  const handleStockOutSelect = async (stockOut) => {
    if (!stockOut) {
      setFormData((prev) => ({
        ...prev,
        selectedStockOut: null,
        orders: [
          {
            productId: null,
            itemName: '',
            unit: null,
            unitPrice: '',
            units: '',
            unitTotalPrice: 0,
          },
        ],
      }));
      return;
    }

    setIsRefLoading(true);
    try {
      let fullDetail = null;
      if (formData.type === STOCK_IN_TYPES.ADJUSTMENT) {
        const response = await stockTakeApi.getById(stockOut.id);
        fullDetail = response?.data || response;
      } else {
        const response = await stockOutApi.getById(stockOut.id);
        fullDetail = response?.data || response;
      }

      const itemsSource =
        fullDetail.stockOutDetails ||
        fullDetail.stockTakeDetails ||
        fullDetail.items ||
        fullDetail.details ||
        [];

      const mappedItems = itemsSource
        .filter((detail) => {
          if (formData.type === STOCK_IN_TYPES.ADJUSTMENT) {
            const diff =
              detail.difference !== undefined
                ? detail.difference
                : detail.actualQty - detail.sysQty;
            return diff < 0;
          }
          return true;
        })
        .map((detail) => {
          const productInfo = products.find((p) => p.id === detail.productId);

          let qty = detail.quantity || 0;
          if (formData.type === STOCK_IN_TYPES.ADJUSTMENT) {
            qty = Math.abs(detail.difference || detail.actualQty - detail.sysQty || 0);
          }

          return {
            productId: detail.productId,
            itemName: productInfo?.name || detail.productName || '',
            unit: units.find((u) => u.id === (productInfo?.unitId || detail.unitId)) || null,
            unitPrice: 0,
            units: qty,
            unitTotalPrice: 0,
          };
        });

      let autoToWarehouse =
        warehouses.find((w) => w.id === fullDetail.warehouseId) || formData.warehouse;
      let autoFromWarehouse = null;
      let autoFromDepartment = null;

      if (formData.type === STOCK_IN_TYPES.TRANSFER) {
        autoFromWarehouse = warehouses.find((w) => w.id === fullDetail.warehouseId) || null;
        autoToWarehouse =
          warehouses.find((w) => w.id === fullDetail.toWarehouseId) || formData.warehouse;
      } else if (formData.type === STOCK_IN_TYPES.RETURN) {
        autoFromDepartment = departments?.find((d) => d.id === fullDetail.departmentId) || null;
        autoToWarehouse =
          warehouses.find((w) => w.id === fullDetail.warehouseId) || formData.warehouse;
      }

      setFormData((prev) => ({
        ...prev,
        selectedStockOut: fullDetail,
        warehouse: autoToWarehouse,
        fromWarehouse: autoFromWarehouse,
        fromDepartment: autoFromDepartment,
        orders: mappedItems.length > 0 ? mappedItems : prev.orders,
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

    const selectedProductIds = formData.orders
      .filter((_, i) => i !== currentIndex)
      .map((order) => order.productId)
      .filter(Boolean);

    return currentProducts.filter((product) => !selectedProductIds.includes(product.id));
  };

  const handleProductSelect = (index, product) => {
    setFormData((prevData) => {
      const updatedOrders = [...prevData.orders];
      if (product) {
        updatedOrders[index] = {
          ...updatedOrders[index],
          productId: product.id,
          itemName: product.name,
          unit: units.find((u) => u.id === product.unitId) || null,
          unitPrice: formData.type === STOCK_IN_TYPES.PURCHASE ? product.purchasePrice || 0 : 0,
          units: 1,
        };
      } else {
        updatedOrders[index] = {
          ...updatedOrders[index],
          productId: null,
          itemName: '',
          unit: null,
          unitPrice: 0,
          units: '',
          unitTotalPrice: 0,
        };
      }
      const totals = calculateTotals(updatedOrders);
      return { ...prevData, orders: updatedOrders, ...totals };
    });
  };

  const handleUnitChange = (index, unit) => {
    setFormData((prevData) => {
      const updatedOrders = [...prevData.orders];
      updatedOrders[index] = { ...updatedOrders[index], unit: unit };
      return { ...prevData, orders: updatedOrders };
    });
  };

  const handleOrderChange = (index, field, value) => {
    setFormData((prevData) => {
      const updatedOrders = [...prevData.orders];
      updatedOrders[index] = { ...updatedOrders[index], [field]: value };
      const totals = calculateTotals(updatedOrders);
      return { ...prevData, orders: updatedOrders, ...totals };
    });
  };

  const handleAddItem = () => {
    setFormData((prevData) => ({
      ...prevData,
      orders: [
        ...prevData.orders,
        { productId: null, itemName: '', unit: null, unitPrice: '', units: '', unitTotalPrice: 0 },
      ],
    }));
  };

  const handleDeleteItem = (index) => {
    setFormData((prevData) => {
      const updatedOrders = prevData.orders.filter((_, i) => i !== index);
      const totals = calculateTotals(updatedOrders);
      return { ...prevData, orders: updatedOrders, ...totals };
    });
  };

  const validateForm = () => {
    const newErrors = {};
    if (!formData.warehouse) newErrors.warehouse = t('Validation.Required');
    if (!formData.requester) newErrors.requester = t('Validation.Required');
    if (formData.type === STOCK_IN_TYPES.PURCHASE && !formData.supplier)
      newErrors.supplier = t('Validation.Required');
    if (formData.type === STOCK_IN_TYPES.TRANSFER && !formData.fromWarehouse)
      newErrors.fromWarehouse = t('Validation.Required');
    if (formData.type === STOCK_IN_TYPES.RETURN && !formData.fromDepartment)
      newErrors.fromDepartment = t('Validation.Required');
    if (!formData.orders.length) newErrors.orders = t('Validation.AtLeastOneItem');
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validateForm()) return;
    const payload = {
      stockInDate: formData.date,
      type: formData.type,
      warehouseId: formData.warehouse?.id,
      requesterId: formData.requester?.id,
      supplierId: formData.supplier?.id,
      fromWarehouseId: formData.fromWarehouse?.id,
      fromDepartmentId: formData.fromDepartment?.id,
      referenceId: formData.selectedStockOut?.code,
      note: formData.note,
      items: formData.orders.map((o) => ({
        productId: o.productId,
        quantity: Number(o.units),
        unitPrice: Number(o.unitPrice),
        unitId: o.unit?.id,
      })),
    };
    await addStockIn(payload);
    setShowAlert(true);
    setTimeout(() => router('/inventory/stock-in/list'), 1000);
  };

  const renderSourceField = () => {
    const commonSize = { xs: 12, sm: 6 };
    switch (formData.type) {
      case STOCK_IN_TYPES.PURCHASE:
        return (
          <Grid item size={commonSize}>
            <CustomFormLabel>{t('Entity.Supplier')}</CustomFormLabel>
            <Autocomplete
              fullWidth
              options={suppliers || []}
              getOptionLabel={(o) => o.name}
              value={formData.supplier}
              onChange={(e, v) => setFormData({ ...formData, supplier: v })}
              renderInput={(p) => <CustomTextField {...p} error={!!errors.supplier} />}
            />
          </Grid>
        );
      case STOCK_IN_TYPES.TRANSFER:
        return (
          <Grid item size={commonSize}>
            <CustomFormLabel>{t('Field.FromWarehouse')}</CustomFormLabel>
            <Autocomplete
              fullWidth
              disabled={!!formData.selectedStockOut}
              options={(warehouses || []).filter((w) => w.id !== formData.warehouse?.id)}
              getOptionLabel={(o) => o.name}
              value={formData.fromWarehouse}
              onChange={(e, v) => setFormData({ ...formData, fromWarehouse: v })}
              renderInput={(p) => <CustomTextField {...p} error={!!errors.fromWarehouse} />}
            />
          </Grid>
        );
      case STOCK_IN_TYPES.RETURN:
        return (
          <Grid item size={commonSize}>
            <CustomFormLabel>{t('Field.FromDepartment')}</CustomFormLabel>
            <Autocomplete
              fullWidth
              disabled={!!formData.selectedStockOut}
              options={departments || []}
              getOptionLabel={(o) => o.name}
              value={formData.fromDepartment}
              onChange={(e, v) => setFormData({ ...formData, fromDepartment: v })}
              renderInput={(p) => <CustomTextField {...p} error={!!errors.fromDepartment} />}
            />
          </Grid>
        );
      case STOCK_IN_TYPES.ADJUSTMENT:
        return (
          <Grid item size={commonSize}>
            <CustomFormLabel>{t('Field.Note')}</CustomFormLabel>
            <CustomTextField
              fullWidth
              value={formData.note}
              onChange={(e) => setFormData({ ...formData, note: e.target.value })}
            />
          </Grid>
        );
      default:
        return null;
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <Box>
        <Stack direction="row" spacing={2} justifyContent="space-between" mb={3}>
          <Typography variant="h5">{t('Menu.StockIn')}</Typography>
          <Box display="flex" gap={1}>
            <Button
              variant="outlined"
              color="error"
              onClick={() => router('/inventory/stock-in/list')}
            >
              {t('Action.Cancel')}
            </Button>
            <Button type="submit" variant="contained" color="primary">
              {t('Action.Add')}
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
                  value={STOCK_IN_TYPES.PURCHASE}
                  control={<Radio />}
                  label={t('StockInType.Purchase')}
                />
                <FormControlLabel
                  value={STOCK_IN_TYPES.TRANSFER}
                  control={<Radio />}
                  label={t('StockInType.Transfer')}
                />
                <FormControlLabel
                  value={STOCK_IN_TYPES.RETURN}
                  control={<Radio />}
                  label={t('StockInType.Return')}
                />
                <FormControlLabel
                  value={STOCK_IN_TYPES.ADJUSTMENT}
                  control={<Radio />}
                  label={t('StockInType.Adjustment')}
                />
              </RadioGroup>
            </FormControl>

            <Fade
              in={[
                STOCK_IN_TYPES.RETURN,
                STOCK_IN_TYPES.TRANSFER,
                STOCK_IN_TYPES.ADJUSTMENT,
              ].includes(formData.type)}
              unmountOnExit
            >
              <Box sx={{ minWidth: { xs: '100%', md: 350 } }}>
                <CustomFormLabel sx={{ mt: 0, mb: 0.5 }}>{t('Field.Reference')}</CustomFormLabel>
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
            <CustomFormLabel>{t('Entity.Requester')}</CustomFormLabel>
            <Autocomplete
              fullWidth
              options={requesters || []}
              getOptionLabel={(o) => o.name}
              value={formData.requester}
              onChange={(e, v) => setFormData({ ...formData, requester: v })}
              renderInput={(p) => <CustomTextField {...p} error={!!errors.requester} />}
            />
          </Grid>
          <Grid item size={{ xs: 12, sm: 6 }}>
            <CustomFormLabel>{t('Field.ToWarehouse')}</CustomFormLabel>
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
            <CustomFormLabel>{t('Field.Date')}</CustomFormLabel>
            <LocalizationProvider dateAdapter={AdapterDateFns}>
              <DatePicker
                format="dd/MM/yyyy"
                enableAccessibleFieldDOMStructure={false}
                value={formData.date ? new Date(formData.date) : null}
                onChange={(newValue) =>
                  setFormData((prev) => ({ ...prev, date: format(newValue, 'yyyy-MM-dd') }))
                }
                slots={{ textField: CustomTextField }}
                slotProps={{ textField: { fullWidth: true } }}
              />
            </LocalizationProvider>
          </Grid>
          {renderSourceField()}
        </Grid>

        <Stack direction="row" justifyContent="space-between" mb={2}>
          <Typography variant="h6">{t('Field.Items')}</Typography>
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
                  <TableCell width="30%">{t('Menu.Product')}</TableCell>
                  <TableCell width="15%">{t('Menu.Unit')}</TableCell>
                  <TableCell width="15%">{t('Field.Price')}</TableCell>
                  <TableCell width="15%">{t('Field.Quantity')}</TableCell>
                  <TableCell width="15%">{t('Field.Total')}</TableCell>
                  <TableCell width="10%">{t('Field.Action')}</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {formData.orders.map((order, index) => (
                  <TableRow key={index}>
                    <TableCell sx={{ verticalAlign: 'top' }}>
                      <Autocomplete
                        options={getAvailableProducts(index)}
                        getOptionLabel={(o) => o.name}
                        value={products.find((p) => p.id === order.productId) || null}
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
                        value={order.unit}
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
                        value={order.unitPrice}
                        disabled={formData.type !== STOCK_IN_TYPES.PURCHASE}
                        onChange={(e) => handleOrderChange(index, 'unitPrice', e.target.value)}
                      />
                    </TableCell>
                    <TableCell sx={{ verticalAlign: 'top' }}>
                      <CustomTextField
                        type="number"
                        value={order.units}
                        onChange={(e) => handleOrderChange(index, 'units', e.target.value)}
                        error={!!errors[`units_${index}`]}
                      />
                    </TableCell>
                    <TableCell>
                      <Typography variant="body1" fontWeight={600}>
                        {formData.type === STOCK_IN_TYPES.PURCHASE
                          ? order.unitTotalPrice.toLocaleString()
                          : '-'}
                      </Typography>
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
        {formData.type === STOCK_IN_TYPES.PURCHASE && (
          <Box p={3} bgcolor="primary.light" mt={3}>
            <Box display="flex" justifyContent="end" gap={3}>
              <Typography variant="body1" fontWeight={600}>
                {t('Field.GrandTotal')}:
              </Typography>
              <Typography variant="body1" fontWeight={600}>
                {formData.grandTotal.toLocaleString()}
              </Typography>
            </Box>
          </Box>
        )}
        <Snackbar
          open={showAlert}
          autoHideDuration={3000}
          onClose={() => setShowAlert(false)}
          anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
        >
          <Alert severity="success">{t('Message.Success')}</Alert>
        </Snackbar>
      </Box>
    </form>
  );
};

export default CreateStockInApp;
