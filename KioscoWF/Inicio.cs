using Entidades;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KioscoWF
{
    public partial class Kiosco : Form
    {
        private Conexion conexion;
        private DataTable productos;
        private DataTable productosGrilla;
        private DataTable categorias;
        private DataTable categoriasGrilla;
        private DataTable estados;
        private DataSet excel;
        private Productos productoSeleccionado;
        DataTable productosAgregados;
        DataTable productosActualizados;
        DataTable productosRechazados;
        public AutoCompleteSource AutoCompleteSource { get; set; }
        public AutoCompleteMode AutoCompleteMode { get; set; }
        public AutoCompleteStringCollection AutoCompleteCustomSource { get; set; }

        public Kiosco()
        {
            InitializeComponent();
            IniciarControles();
        }



        //INICIO DEL PROGRAMA
        private void IniciarControles()
        {
            conexion = new Conexion();
            excel = new DataSet();

            IniciarControlesCategorias();
            IniciarControlesProductos();
            IniciarControlesExcel();

            estados = conexion.ObtenerDataTable("exec EstadosSeleccionar");

            ddlEstadoCategoriasAgregar.DataSource = estados;
            ddlEstadoCategoriasAgregar.DisplayMember = "Estado";
            ddlEstadoCategoriasAgregar.ValueMember = "IdEstado";

            ddlEstadoProductosAgregar.DataSource = estados;
            ddlEstadoProductosAgregar.DisplayMember = "Estado";
            ddlEstadoProductosAgregar.ValueMember = "IdEstado";
        }
        public void IniciarControlesProductos()
        {
            productos = conexion.ObtenerDataTable("exec ProductosSeleccionar");

            AutoCompleteStringCollection autocompleteProductos = new AutoCompleteStringCollection();
            foreach (DataRow row in productos.Rows)
            {
                autocompleteProductos.Add(row["Producto"].ToString());
            }

            ddlProductoInicio.AutoCompleteCustomSource = autocompleteProductos;
            ddlProductoInicio.AutoCompleteMode = AutoCompleteMode.Suggest;
            ddlProductoInicio.AutoCompleteSource = AutoCompleteSource.CustomSource;

            ClickBuscarProductosListar(null, EventArgs.Empty);
            GrillaProductos.ReadOnly = true;
        }
        public void IniciarControlesCategorias()
        {
            categorias = conexion.ObtenerDataTable("exec CategoriasSeleccionar");

            ddlCategoriaProductosAgregar.DataSource = categorias;
            ddlCategoriaProductosAgregar.DisplayMember = "Categoria";
            ddlCategoriaProductosAgregar.ValueMember = "IdCategoria";
            ddlCategoriaProductosAgregar.SelectedValue = 1.ToString();

            DataRow dr = categorias.NewRow();
            dr["Categoria"] = "Seleccione una opcion";
            dr["IdCategoria"] = 0;

            categorias.Rows.InsertAt(dr, 0);


            ddlCategoriaProductosListar.DataSource = categorias;
            ddlCategoriaProductosListar.DisplayMember = "Categoria";
            ddlCategoriaProductosListar.ValueMember = "IdCategoria";
           
            ddlCategoriaProductosAgregar.SelectedValue = 0.ToString();

            ClickBuscarCategoriasListar(null, EventArgs.Empty);
            GrillaCategorias.ReadOnly = true;
        }
        public void IniciarControlesExcel()
        {
            productosAgregados = new DataTable();
            productosActualizados = new DataTable();
            productosRechazados = new DataTable();

            productosAgregados.Columns.Add("IdProducto");
            productosAgregados.Columns.Add("Producto");
            productosAgregados.Columns.Add("Precio");
            productosAgregados.Columns.Add("IdCategoria");
            productosAgregados.Columns.Add("Detalle");
            productosAgregados.Columns.Add("Codigo");
            productosAgregados.Columns.Add("Stock");

            productosActualizados.Columns.Add("IdProducto");
            productosActualizados.Columns.Add("Producto");
            productosActualizados.Columns.Add("Precio");
            productosActualizados.Columns.Add("IdCategoria");
            productosActualizados.Columns.Add("Detalle");
            productosActualizados.Columns.Add("Codigo");
            productosActualizados.Columns.Add("Stock");

            productosRechazados.Columns.Add("IdProducto");
            productosRechazados.Columns.Add("Producto");
            productosRechazados.Columns.Add("Precio");
            productosRechazados.Columns.Add("IdCategoria");
            productosRechazados.Columns.Add("Detalle");
            productosRechazados.Columns.Add("Codigo");
            productosRechazados.Columns.Add("Stock");       
        }



        //PAGINA INICIO
        private void PresionarTeclaPaginas(object sender, KeyEventArgs e)
        {
            if (Paginas.SelectedTab.Text == "Inicio")
            {
                if (e.KeyValue == (char)Keys.Enter)
                {
                    bool cargarProducto = false;

                    if (txtCodigoInicio.Text != string.Empty)
                    {
                        foreach (DataRow row in productos.Rows)
                        {
                            if (row["Codigo"].ToString() == txtCodigoInicio.Text)
                            {
                                Productos p = ObtenerProducto("Codigo", row["Codigo"].ToString());
                                CargarProducto(p);
                                txtStockMostrar.Focus();

                                cargarProducto = true;
                                break;
                            }
                        }
                        if (!cargarProducto)
                        {
                            VaciarTextBox();
                            MessageBox.Show("Error al cargar producto.");
                        }
                    }
                }
                else if(e.KeyValue == (char)Keys.Up)
                {
                    if (!ddlProductoInicio.Focused)
                    {
                        Int32.TryParse(txtStockMostrar.Text, out int stock);
                        stock++;
                        txtStockMostrar.Text = (stock).ToString();
                        ModificarStock(stock);
                    }
                }
                else if(e.KeyValue == (char)Keys.Down)
                {
                    if (!ddlProductoInicio.Focused)
                    {
                        Int32.TryParse(txtStockMostrar.Text, out int stock);
                        stock--;
                        txtStockMostrar.Text = (stock).ToString();
                        ModificarStock(stock);
                    }
                }
            }
        }
        private void SeleccionarProductoInicio(object sender, EventArgs e)
        {
            foreach (DataRow row in productos.Rows)
            {
                if (row["Producto"].ToString() == ddlProductoInicio.SelectedText)
                {
                    Productos p = ObtenerProducto("Producto", row["Producto"].ToString());
                    CargarProducto(p);
                    txtStockMostrar.Focus();
                    break;
                }
            }
        }
        private Productos ObtenerProducto(string rowNombre, string dato)
        {
            Productos p = new Productos();

            foreach (DataRow row in productos.Rows)
            {
                if (row[rowNombre].ToString() == dato)
                {
                    p.IdProducto = Convert.ToInt32(row["IdProducto"]);
                    p.Codigo = row["Codigo"].ToString();
                    p.Producto = row["Producto"].ToString();
                    p.Precio = Convert.ToDecimal(row["Precio"]);
                    p.Categoria.IdCategoria = Convert.ToInt32(row["IdCategoria"]);
                    p.Detalle = row["Detalle"].ToString();
                    p.IdEstado = Convert.ToInt32(row["IdEstado"]);
                    p.Stock = Convert.ToInt32(row["Stock"]);

                    foreach (DataRow row2 in categorias.Rows)
                    {
                        if (row2["IdCategoria"].ToString() == p.Categoria.IdCategoria.ToString())
                        {
                            p.Categoria.Categoria = row2["Categoria"].ToString();
                        }
                    }

                    break;
                }
            }
            return p;
        }
        private void CargarProducto(Productos p)
        {
            txtCodigoMostrar.Text = p.Codigo;
            txtProductoMostrar.Text = p.Producto;

            try
            {
                txtPrecioMostrar.Text = p.Precio.ToString();
                txtStockMostrar.Text = p.Stock.ToString();
            }
            catch
            {
                MessageBox.Show("El precio o el stock del producto se encuentra en un formato invalido");
            }

            VaciarTextBox();
            productoSeleccionado = p;
            txtStockMostrar.ReadOnly = false;
        }
        private void VaciarTextBox()
        {
            txtCodigoInicio.Text = "";
            ddlProductoInicio.Text = "";
        }
        private void txtStockMostrar_TextChanged(object sender, EventArgs e)
        {
            if (txtStockMostrar.Focused == true)
            {
                Int32.TryParse(txtStockMostrar.Text, out int stock);
                ModificarStock(stock);
            }
        }
        private void ModificarStock(int stock)
        {
            string pIdProducto = GenerarParametros("IdProducto", productoSeleccionado.IdProducto.ToString(), "int"); //Aca va IdProducto
            string pStock = GenerarParametros("Stock", stock.ToString(), "int"); //Aca va IdProducto

            string sql = "exec ProductosModificarStock " + pIdProducto + ',' + pStock;

            conexion.EjecutarQuery(sql);
            Program.kiosco.ddlCategoriaProductosListar.SelectedValue = 0;
            Program.kiosco.IniciarControlesProductos();
        }



        //PAGINA PRODUCTOS
        private void ClickBuscarProductosListar(object sender, EventArgs e)
        {
            string producto = GenerarParametros("Producto", txtProductoProductosListar.Text, "string");
            string categoria = GenerarParametros("IdCategoria", Convert.ToString(ddlCategoriaProductosListar.SelectedValue), "int");
            string codigo = GenerarParametros("Codigo", txtCodigoProductosListar.Text, "string");

            string sql = "exec ProductosSeleccionarGrilla " + producto + "," + categoria + "," + codigo;
            productosGrilla = conexion.ObtenerDataTable(sql);

            productosGrilla.Columns.Add("Modificar");
            productosGrilla.Columns.Add("Baja");

            foreach (DataRow row in productosGrilla.Rows)
            {
                row["Modificar"] = "Modificar";
                row["Baja"] = "Baja";
            }

            GrillaProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            GrillaProductos.DataSource = productosGrilla;
            GrillaProductos.Columns["IdCategoria"].Visible = false;
        }
        private void AccionGrillaProductos(object sender, DataGridViewCellEventArgs e)
        {
            Productos p = ObtenerProducto("Producto", productosGrilla.Rows[e.RowIndex]["Producto"].ToString());

            if (productosGrilla.Columns[e.ColumnIndex].ColumnName == "Modificar")
            {
                ModificarProductos modificar = new ModificarProductos(p);
                modificar.Show();
            }
            if (productosGrilla.Columns[e.ColumnIndex].ColumnName == "Baja")
            {
                DialogResult respuesta = MessageBox.Show("¿Esta seguro que desea dar de baja " + p.Producto + "?", "Mercante", MessageBoxButtons.YesNo);
                if (respuesta == DialogResult.Yes)
                {
                    string Producto = txtCategoriaCategoriasAgregar.Text;

                    string pProducto = GenerarParametros("IdProducto", p.IdProducto.ToString(), "int"); //Aca va IdProducto

                    string sql = "exec ProductosBorrar " + pProducto;

                    conexion.EjecutarQuery(sql);

                    MessageBox.Show(p.Producto + " ha sido eliminado con exito.");

                    IniciarControlesProductos();
                }
            }
        }
        private void ClickAgregarProductosAgregar(object sender, EventArgs e)
        {
            try
            {
                bool existeCodigo = false;
                bool existeProducto = false;
                string nombreCodigo = "";

                decimal precio;
                string producto = txtProductoProductosAgregar.Text;
                int categoria = Convert.ToInt32(ddlCategoriaProductosAgregar.SelectedValue);
                int estado = Convert.ToInt32(ddlEstadoProductosAgregar.SelectedValue);
                string detalle = txtDetalleProductosAgregar.Text;
                string codigo = txtCodigoProductosAgregar.Text;
                bool stockFlag = Int32.TryParse(txtStockProductosAgregar.Text, out int stock);

                if (txtPrecioProductosAgregar.Text.Contains("."))
                {
                    precio = Convert.ToDecimal(txtPrecioProductosAgregar.Text.Replace(".", ","));
                }
                else
                {
                    precio = Convert.ToDecimal(txtPrecioProductosAgregar.Text);
                }

                if (producto != string.Empty && estado > 0 && precio > 0 && categoria > 0 && stockFlag && stock >= 0)
                {
                    foreach (DataRow row in productos.Rows)
                    {
                        if (row["Codigo"].ToString() != "")
                        {
                            if (row["Codigo"].ToString() == codigo)
                            {
                                nombreCodigo = row["Producto"].ToString();
                                existeCodigo = true;
                                break;
                            }
                        }
                        if (row["Producto"].ToString() != "")
                        {
                            if (row["Producto"].ToString() == producto)
                            {
                                nombreCodigo = row["Producto"].ToString();
                                existeProducto = true;
                                break;
                            }
                        }
                    }

                    if (!existeCodigo && !existeProducto)
                    {
                        string pProducto = GenerarParametros("Producto", producto, "string");
                        string pPrecio = GenerarParametros("Precio", precio.ToString(), "decimal");
                        string pCategoria = GenerarParametros("IdCategoria", categoria.ToString(), "int");
                        string pEstado = GenerarParametros("IdEstado", estado.ToString(), "int");
                        string pDetalle = GenerarParametros("Detalle", detalle, "string");
                        string pCodigo = GenerarParametros("codigo", codigo, "string");
                        string pStock = GenerarParametros("stock", stock.ToString(), "int");

                        string sql = "exec ProductosInsertar " + pProducto + "," + pPrecio + "," + pCategoria
                            + "," + pDetalle + "," + pEstado + "," + pCodigo + "," + pStock;

                        conexion.EjecutarQuery(sql);

                        MessageBox.Show(producto + " agregado con exito.");

                        ddlCategoriaProductosListar.SelectedValue = 0;
                        IniciarControlesProductos();

                        txtProductoProductosAgregar.Text = "";
                        txtPrecioProductosAgregar.Text = "";
                        ddlCategoriaProductosAgregar.SelectedValue = 1;
                        ddlEstadoProductosAgregar.SelectedValue = 1;
                        txtDetalleProductosAgregar.Text = "";
                        txtCodigoProductosAgregar.Text = "";
                    }
                    else if (existeCodigo)
                    {
                        MessageBox.Show("Error, el codigo ingresado ya pertenece a " + nombreCodigo + ".");
                    }
                    else if (existeProducto)
                    {
                        MessageBox.Show("Error, el nombre ingresado ya pertenece a otro producto.");
                    }
                }
                else
                {
                    MessageBox.Show("Error, verifique que los valores ingresados.");
                }

            }
            catch
            {
                MessageBox.Show("Error, verifique que los valores ingresados.");
            }
        }
        private void btnExportarExcelProductos_Click(object sender, EventArgs e)
        {
            if (productos.Rows.Count > 0)
            {
                DataTable excel = new DataTable();
                excel.Columns.Add("IdProducto");
                excel.Columns.Add("Producto");
                excel.Columns.Add("Precio");
                excel.Columns.Add("IdCategoria");
                excel.Columns.Add("Detalle");
                excel.Columns.Add("Codigo");
                excel.Columns.Add("Stock");

                foreach(DataRow row in productosGrilla.Rows)
                {
                    excel.Rows.Add(row["IdProducto"], row["Producto"], row["Precio"], row["IdCategoria"], row["Detalle"], row["Codigo"], row["Stock"]);
                }

                var lines = new List<string>();

                string[] columnNames = excel.Columns
                    .Cast<DataColumn>()
                    .Select(column => column.ColumnName)
                    .ToArray();

                var header = string.Join(",", columnNames.Select(name => $"\"{name}\""));
                lines.Add(header);

                var valueLines = excel.AsEnumerable()
                    .Select(row => string.Join(",", row.ItemArray.Select(val => $"\"{val}\"")));

                lines.AddRange(valueLines);

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = "Productos";
                saveFileDialog.Filter = "Excel Worbook|*.xlsx";
                saveFileDialog.Title = "Guardar Excel";

                saveFileDialog.ShowDialog();

                if (saveFileDialog.FileName != "")
                {
                    File.WriteAllLines(saveFileDialog.FileName, lines);
                }
            }
        }


        //PAGINA CATEGORIAS
        private void ClickBuscarCategoriasListar(object sender, EventArgs e)
        {
            string categoria = GenerarParametros("Categoria", txtCategoriaCategoriasListar.Text, "string");

            string sql = "exec CategoriasSeleccionarGrilla " + categoria;
            categoriasGrilla = conexion.ObtenerDataTable(sql);

            categoriasGrilla.Columns.Add("Modificar");
            categoriasGrilla.Columns.Add("Baja");
            foreach (DataRow row in categoriasGrilla.Rows)
            {
                row["Modificar"] = "Modificar";
                row["Baja"] = "Baja";
            }

            GrillaCategorias.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            GrillaCategorias.DataSource = categoriasGrilla;
        }
        private void AccionGrillaCategorias(object sender, DataGridViewCellEventArgs e)
        {
            Categorias c = new Categorias();
            c.IdCategoria = Convert.ToInt32(categoriasGrilla.Rows[e.RowIndex]["IdCategoria"].ToString());
            c.Categoria = categoriasGrilla.Rows[e.RowIndex]["Categoria"].ToString();
            c.Detalle = categoriasGrilla.Rows[e.RowIndex]["Detalle"].ToString();
            //p.IdEstado = productos.Rows[e.RowIndex]["Codigo"].ToString();

            if (categoriasGrilla.Columns[e.ColumnIndex].ColumnName == "Modificar")
            {
                if (c.IdCategoria == 1)
                {
                    MessageBox.Show("No se puede modificar la Categoria " + c.Categoria + ".");
                    return;

                }

                ModificarCategoria modificar = new ModificarCategoria(c);
                modificar.Show();
                IniciarControlesCategorias();
            }
            if (categoriasGrilla.Columns[e.ColumnIndex].ColumnName == "Baja")
            {

                if (c.IdCategoria == 1)
                {
                    MessageBox.Show("No se puede dar de baja la Categoria " + c.Categoria + ".");
                    return;

                }
                DialogResult respuesta = MessageBox.Show("¿Esta seguro que desea dar de baja " + c.Categoria + "?", "Mercante", MessageBoxButtons.YesNo);
                if (respuesta == DialogResult.Yes)
                {

                    string Categoria = txtCategoriaCategoriasAgregar.Text;

                    string pCategoria = GenerarParametros("IdCategoria", c.IdCategoria.ToString(), "int"); //Aca va IdCategoria

                    string sql = "exec CategoriasBorrar " + pCategoria;

                    conexion.EjecutarQuery(sql);
                    IniciarControlesCategorias();
                    IniciarControlesProductos();
                }
            }
        }
        private void ClickAgregarCategoriasAgregar(object sender, EventArgs e)
        {
            string nombre = txtCategoriaCategoriasAgregar.Text;
            int estado = Convert.ToInt32(ddlEstadoCategoriasAgregar.SelectedValue);
            string detalle = txtDetalleCategoriasAgregar.Text;

            if (nombre != string.Empty && estado != -1)
            {
                string pCategoria = GenerarParametros("Categoria", nombre, "string");
                string pEstado = GenerarParametros("IdEstado", estado.ToString(), "int");
                string pDetalle = GenerarParametros("Detalle", detalle, "string");

                string sql = "exec CategoriasInsertar " + pCategoria + "," + pEstado + "," + pDetalle;

                conexion.EjecutarQuery(sql);
                MessageBox.Show("Categoria agregada con exito.");
                IniciarControlesCategorias();
                txtCategoriaCategoriasAgregar.Text = "";
                ddlEstadoCategoriasAgregar.SelectedValue = 1;
                txtDetalleCategoriasAgregar.Text = "";
            }
            else
            {
                MessageBox.Show("Error al agregar categoria, revisa bien los datos.");
            }
        }



        //PAGINA EXCEL
        private void btnImportarExcel_Click(object sender, EventArgs e)
        {
            OpenFileDialog oOpenFileDialog = new OpenFileDialog();
            oOpenFileDialog.Filter = "Excel Worbook|*.xlsx";

            if (oOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                grillaExcel.DataSource = null;

                txtRutaExcel.Text = oOpenFileDialog.FileName;

                FileStream fsSource = new FileStream(oOpenFileDialog.FileName, FileMode.Open, FileAccess.Read);

                IExcelDataReader reader = ExcelReaderFactory.CreateReader(fsSource);

                excel = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true
                    }
                });

                reader.Close();

                DataColumnCollection columns = excel.Tables[0].Columns;

                if (columns[0].ColumnName != "IdProducto"
                    || columns[1].ColumnName != "Producto"
                    || columns[2].ColumnName != "Precio"
                    || columns[3].ColumnName != "IdCategoria"
                    || columns[4].ColumnName != "Detalle"
                    || columns[5].ColumnName != "Codigo"
                    || columns[6].ColumnName != "Stock"
                    )
                {
                    MessageBox.Show("Error. Las columnas no coinciden con el formato adecuado.");
                    CancelarExcel();
                    return;
                }

                grillaExcel.DataSource = excel.Tables[0];
                btnSubirExcel.Enabled = true;
                btnSubirExcel.Text = "Cargar";
                lblProductosExcel.Visible = false;
                btnCancelarExcel.Visible = false;
                ddlProductosExcel.Visible = false;
            }
        }
        private void btnSubirExcel_Click(object sender, EventArgs e)
        {
            if (btnSubirExcel.Text == "Cargar")
            {
                CargarExcel();
            }
            else
            {
                SubirExcel();
            }
        }
        private void btnCancelarExcel_Click(object sender, EventArgs e)
        {
            CancelarExcel();
        }
        private void ddlProductosExcel_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (ddlProductosExcel.SelectedIndex)
            {
                case 0:
                    grillaExcel.DataSource = productosAgregados;
                    break;
                case 1:
                    grillaExcel.DataSource = productosActualizados;
                    break;
                case 2:
                    grillaExcel.DataSource = productosRechazados;
                    break;
            }
        }
        private void btnPlantillaExcel_Click(object sender, EventArgs e)
        {
            DataTable excel = new DataTable();
            excel.Columns.Add("IdProducto");
            excel.Columns.Add("Producto");
            excel.Columns.Add("Precio");
            excel.Columns.Add("IdCategoria");
            excel.Columns.Add("Detalle");
            excel.Columns.Add("Codigo");
            excel.Columns.Add("Stock");

            var lines = new List<string>();

            string[] columnNames = excel.Columns
                .Cast<DataColumn>()
                .Select(column => column.ColumnName)
                .ToArray();

            var header = string.Join(",", columnNames.Select(name => $"\"{name}\""));
            lines.Add(header);

            var valueLines = excel.AsEnumerable()
                .Select(row => string.Join(",", row.ItemArray.Select(val => $"\"{val}\"")));

            lines.AddRange(valueLines);

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "Productos";
            saveFileDialog.Filter = "Excel Worbook|*.xlsx";
            saveFileDialog.Title = "Guardar Excel";

            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                File.WriteAllLines(saveFileDialog.FileName, lines);
            }
        }
        private void CargarExcel()
        {
            DataTable data = (DataTable)(grillaExcel.DataSource);
            bool existeCodigo;
            bool existeProducto;
            Productos producto;
            productosAgregados.Rows.Clear();
            productosActualizados.Rows.Clear();
            productosRechazados.Rows.Clear();

            foreach (DataRow row in data.Rows)
            {
                existeCodigo = false;
                existeProducto = false;
                producto = new Productos();

                producto.Producto = row["Producto"].ToString();
                producto.Detalle = row["Detalle"].ToString();
                producto.Codigo = row["Codigo"].ToString();

                try
                {
                    producto.Precio = Convert.ToDecimal(row["Precio"].ToString());
                    producto.Categoria.IdCategoria = Convert.ToInt32(row["IdCategoria"].ToString());
                    producto.Stock = Convert.ToInt32(row["Stock"].ToString());

                    if (producto.Producto != string.Empty && producto.Precio >= 0 && producto.Categoria.IdCategoria > 0 && producto.Stock >= 0)
                    {
                        foreach (DataRow row2 in productos.Rows)
                        {
                            if (row2["Producto"].ToString() != "")
                            {
                                if (row2["Producto"].ToString() == producto.Producto)
                                {
                                    producto.IdProducto = Convert.ToInt32(row2["IdProducto"].ToString());
                                    existeProducto = true;
                                    break;
                                }
                            }
                            if (row2["Codigo"].ToString() != "")
                            {
                                if (row2["Codigo"].ToString() == producto.Codigo)
                                {
                                    existeCodigo = true;
                                    break;
                                }
                            }
                        }

                        if (existeCodigo)
                        {
                            productosRechazados.Rows.Add(row["IdProducto"], row["Producto"], row["Precio"], row["IdCategoria"], row["Detalle"], row["Codigo"], row["Stock"]);
                        }
                        else if (existeProducto)
                        {
                            row["IdProducto"] = producto.IdProducto;
                            productosActualizados.Rows.Add(row["IdProducto"], row["Producto"], row["Precio"], row["IdCategoria"], row["Detalle"], row["Codigo"], row["Stock"]);
                        }
                        else
                        {
                            productosAgregados.Rows.Add(row["IdProducto"], row["Producto"], row["Precio"], row["IdCategoria"], row["Detalle"], row["Codigo"], row["Stock"]);
                        }
                    }
                    else
                    {
                        productosRechazados.Rows.Add(row["IdProducto"], row["Producto"], row["Precio"], row["IdCategoria"], row["Detalle"], row["Codigo"], row["Stock"]);
                    }
                }
                catch
                {
                    productosRechazados.Rows.Add(row["IdProducto"], row["Producto"], row["Precio"], row["IdCategoria"], row["Detalle"], row["Codigo"], row["Stock"]);
                }
            }

            btnSubirExcel.Text = "Subir";
            lblProductosExcel.Visible = true;
            btnCancelarExcel.Visible = true;
            ddlProductosExcel.Visible = true;
            ddlProductosExcel.SelectedIndex = 0;
        }
        private void SubirExcel()
        {
            DialogResult respuesta = MessageBox.Show("Se cargaran estos productos en el sistema ¿Desea continuar?", "Negocio", MessageBoxButtons.YesNo);

            if (respuesta == DialogResult.Yes)
            {
                foreach (DataRow row in productosAgregados.Rows)
                {
                    string pProducto = GenerarParametros("Producto", row["Producto"].ToString(), "string");
                    string pPrecio = GenerarParametros("Precio", row["Precio"].ToString(), "decimal");
                    string pCategoria = GenerarParametros("IdCategoria", row["IdCategoria"].ToString(), "int");
                    string pEstado = GenerarParametros("IdEstado", 1.ToString(), "int");
                    string pDetalle = GenerarParametros("Detalle", row["Detalle"].ToString(), "string");
                    string pCodigo = GenerarParametros("codigo", row["Codigo"].ToString(), "string");
                    string pStock = GenerarParametros("stock", row["Stock"].ToString(), "int");

                    string sql = "exec ProductosInsertar " + pProducto + "," + pPrecio + "," + pCategoria
                        + "," + pDetalle + "," + pEstado + "," + pCodigo + "," + pStock;

                    conexion.EjecutarQuery(sql);
                }

                foreach (DataRow row in productosActualizados.Rows)
                {
                    string pIdProducto = GenerarParametros("IdProducto", row["IdProducto"].ToString(), "int");
                    string pProducto = GenerarParametros("Producto", row["Producto"].ToString(), "string");
                    string pPrecio = GenerarParametros("Precio", row["Precio"].ToString(), "decimal");
                    string pCategoria = GenerarParametros("IdCategoria", row["IdCategoria"].ToString(), "int");
                    string pEstado = GenerarParametros("IdEstado", 1.ToString(), "int");
                    string pDetalle = GenerarParametros("Detalle", row["Detalle"].ToString(), "string");
                    string pCodigo = GenerarParametros("codigo", row["Codigo"].ToString(), "string");
                    string pStock = GenerarParametros("stock", row["Stock"].ToString(), "int");

                    string sql = "exec ProductosModificar " + pIdProducto + ',' + pProducto + ',' + pPrecio
                        + ',' + pCategoria + ',' + pDetalle + ',' + pCodigo + ',' + pEstado + ',' + pStock;

                    conexion.EjecutarQuery(sql);
                }

                MessageBox.Show("¡Productos cargados con éxito!");
                IniciarControlesProductos();
                CancelarExcel();
            }
        }
        private void CancelarExcel()
        {
            btnSubirExcel.Text = "Cargar";
            lblProductosExcel.Visible = false;
            btnCancelarExcel.Visible = false;
            ddlProductosExcel.Visible = false;
            txtRutaExcel.Text = string.Empty;
            grillaExcel.DataSource = null;
            btnSubirExcel.Enabled = false;
        }



        //FUNCIONES GENERALES
        private string GenerarParametros(string nombre, string valor, string tipo)
        {
            string parametros = "@";

            if (tipo == "string")
            {
                parametros += nombre + "='" + valor + "' ";
            }
            else if (tipo == "int")
            {
                parametros += nombre + "=" + valor + " ";
            }
            else if (tipo == "decimal")
            {
                if (valor.Contains(","))
                {
                    parametros += nombre + "=" + valor.Replace(",", ".") + " ";
                }
                else
                {
                    parametros += nombre + "=" + valor + " ";
                }
            }
            else if (tipo == "date")
            {
                string[] strs = valor.Split('/');
                if (strs[1].Length == 1)
                {
                    strs[1] = "0" + strs[1];
                }

                if (strs[0].Length == 1)
                {
                    strs[0] = "0" + strs[0];
                }
                parametros += nombre + "='" + strs[2] + strs[1] + strs[0] + "' ";
            }
            return parametros;
        }
        private void CambioPestañaProductos(object sender, EventArgs e)
        {
            ddlCategoriaProductosAgregar.SelectedValue = 1;
            ddlCategoriaProductosListar.SelectedValue = 1;
        }
        private void CambioPestañaPaginas(object sender, EventArgs e)
        {
            ddlEstadoProductosAgregar.SelectedValue = 1;
            ddlEstadoCategoriasAgregar.SelectedValue = 1;
        }
    }
}
