using Entidades;
using iTextSharp.text;
using iTextSharp.text.pdf;
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
        Conexion conexion;
        public DataTable productos;
        public DataTable productosGrilla;
        public DataTable categorias;
        public DataTable categoriasGrilla;
        public DataTable estados;
        public DataTable inicio;
        public DataTable ventas;
        public List<Ventas> ventasViejas;
        public List<Ventas> ventasModificadas;
        public decimal total = 0;
        public List<Productos> productosVendidos = new List<Productos>();
        DateTime fechaExportarFiltro;
        DateTime fechaHastaExportarFiltro;

        public AutoCompleteSource AutoCompleteSource { get; set; }
        public AutoCompleteMode AutoCompleteMode { get; set; }
        public AutoCompleteStringCollection AutoCompleteCustomSource { get; set; }

        public Kiosco()
        {
            InitializeComponent();
            //IniciarControles();
        }

        private void IniciarControles()
        {
            conexion = new Conexion();
            inicio = new DataTable();
            ventas = new DataTable();
            ventasModificadas = new List<Ventas>();

            IniciarControlesCategorias();
            IniciarControlesProductos();

            estados = conexion.ObtenerDataTable("exec EstadosSeleccionar");

            ddlEstadoCategoriasAgregar.DataSource = estados;
            ddlEstadoCategoriasAgregar.DisplayMember = "Estado";
            ddlEstadoCategoriasAgregar.ValueMember = "IdEstado";

            ddlEstadoProductosAgregar.DataSource = estados;
            ddlEstadoProductosAgregar.DisplayMember = "Estado";
            ddlEstadoProductosAgregar.ValueMember = "IdEstado";

            inicio.Columns.Add("#");
            inicio.Columns.Add("Nombre");
            inicio.Columns.Add("Categoria");
            inicio.Columns.Add("Precio");
            inicio.Columns.Add("Cantidad");
            inicio.Columns.Add("Borrar");

            ventas.Columns.Add("Nombre");
            ventas.Columns.Add("Cantidad");
            ventas.Columns.Add("Precio");
            ventas.Columns.Add("Precio Total");
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


        private void btnAceptarInicio_Click(object sender, EventArgs e)
        {
            InsertarVentas();
        }
        private void btnCancelarInicio_Click(object sender, EventArgs e)
        {
            VaciarVentas();
        }

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
        }
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

               

                if (txtPrecioProductosAgregar.Text.Contains("."))
                {
                    precio = Convert.ToDecimal(txtPrecioProductosAgregar.Text.Replace(".", ","));
                }
                else
                {
                    precio = Convert.ToDecimal(txtPrecioProductosAgregar.Text);
                }

                if (producto != string.Empty && estado > 0 && precio > 0 && categoria > 0)
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

                        string sql = "exec ProductosInsertar " + pProducto + "," + pPrecio + "," + pCategoria
                            + "," + pDetalle + "," + pEstado + "," + pCodigo;

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
                    else if(existeCodigo)
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

        private void btnFiltrarExportarPDF_Click(object sender, EventArgs e)
        {
            ventas.Rows.Clear();
            fechaExportarFiltro = txtFechaExportarPDF.Value;
            fechaHastaExportarFiltro = txtFechaHastaExportarPDF.Value;

            string pFecha1;
            string pFechaHasta1;

            if (fechaExportarFiltro.ToString() == string.Empty)
            {
                MessageBox.Show("Es obligatorio ingresar una fecha.");
                return;
            }
            else
            {
                for (int i = 1; i < categoriasGrilla.Rows.Count; i++)
                {
                    DataRow row = categoriasGrilla.Rows[i];

                    string pFecha = GenerarParametros("Fecha", fechaExportarFiltro.ToShortDateString(), "date");
                    string pFechaHasta = GenerarParametros("FechaHasta", fechaHastaExportarFiltro.ToShortDateString(), "date");
                    string pIdCategoria = GenerarParametros("IdCategoria", row["IdCategoria"].ToString(), "int");

                    string sql = "EXEC VentasSeleccionar " + pFecha + ", " + pFechaHasta + ", " + pIdCategoria;
                    DataTable dt = conexion.ObtenerDataTable(sql);

                    int x = 0;

                    for (int j = x; j < dt.Rows.Count; j++)
                    {
                        DataRow row2 = dt.Rows[j];
                        if (row2[2].ToString() == string.Empty)
                        {
                            ventas.Rows.Add(row2[0], "Cantidad", "Precio", "Precio Total");
                        }
                        else
                        {
                            ventas.Rows.Add(row2[0], row2[2], row2[1], row2[3]);
                        }
                    }
                }
                DataRow row1 = categoriasGrilla.Rows[0];

                pFecha1 = GenerarParametros("Fecha", fechaExportarFiltro.ToShortDateString(), "date");
                pFechaHasta1 = GenerarParametros("FechaHasta", fechaHastaExportarFiltro.ToShortDateString(), "date");
                string pIdCategoria1 = GenerarParametros("IdCategoria", row1["IdCategoria"].ToString(), "int");

                string sql1 = "EXEC VentasSeleccionar " + pFecha1 + ", " + pFechaHasta1 + ", " + pIdCategoria1;
                DataTable dt1 = conexion.ObtenerDataTable(sql1);

                foreach (DataRow row2 in dt1.Rows)
                {
                    if (row2[2].ToString() == string.Empty)
                    {
                        ventas.Rows.Add(row2[0], "Cantidad", "Precio", "Precio Total");
                    }
                    else
                    {
                        ventas.Rows.Add(row2[0], row2[2], row2[1], row2[3]);

                    }
                }
            }

            DataTable totalVentas = conexion.ObtenerDataTable("EXEC VentasSeleccionarTotal " + pFecha1 + ", " + pFechaHasta1);

            ventas.Rows.Add("", "", "Total", totalVentas.Rows[0][0]);


            GrillaExportarPDF.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            GrillaExportarPDF.DataSource = ventas;
            GrillaExportarPDF.ReadOnly = true;

            for (int i = 0; i < GrillaExportarPDF.Rows.Count - 1; i++)
            {
                DataGridViewRow gvRow = GrillaExportarPDF.Rows[i];

                foreach (DataGridViewCell cell in gvRow.Cells)
                {
                    if (cell.ColumnIndex > 0)
                    {
                        try
                        {
                            if (cell.ColumnIndex > 1)
                            {
                                Convert.ToDecimal(cell.Value.ToString().Substring(1));
                            }
                            else
                            {
                                Convert.ToInt32(cell.Value);
                            }
                            cell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                        catch
                        {

                        }
                    }
                }
            }

        }

        private void btnExportarExportarPDF_Click(object sender, EventArgs e)
        {
            if (ventas.Rows.Count > 0)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "PDF (*.pdf)|*.pdf";
                sfd.FileName = "Ventas - " + fechaExportarFiltro.ToString("dd'-'MM'-'yyyy") + ".pdf";
                bool fileError = false;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(sfd.FileName))
                    {
                        try
                        {
                            File.Delete(sfd.FileName);
                        }
                        catch (IOException ex)
                        {
                            fileError = true;
                            MessageBox.Show("It wasn't possible to write the data to the disk." + ex.Message);
                        }
                    }
                    if (!fileError)
                    {
                        try
                        {
                            PdfPTable pdfTable = new PdfPTable(GrillaExportarPDF.Columns.Count);
                            pdfTable.DefaultCell.Padding = 3;
                            pdfTable.WidthPercentage = 100;
                            pdfTable.HorizontalAlignment = Element.ALIGN_LEFT;

                            foreach (DataGridViewColumn column in GrillaExportarPDF.Columns)
                            {
                                PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText));
                                cell.BorderWidth = 1.5F;
                                pdfTable.AddCell(cell);
                            }

                            for(int i = 0; i < GrillaExportarPDF.Rows.Count -1; i++)
                            {
                                if (i == GrillaExportarPDF.Rows.Count - 2)
                                {
                                    DataGridViewRow gvRowFinal = GrillaExportarPDF.Rows[i];
                                    DataRow dRowFinal = ventas.Rows[i];


                                    for (int j = 0; j < gvRowFinal.Cells.Count; j++)
                                    {
                                        DataGridViewCell cell = gvRowFinal.Cells[j];
                                        
                                        PdfPCell cellRow = new PdfPCell(new Phrase(cell.Value.ToString(), FontFactory.GetFont(FontFactory.HELVETICA_BOLD)));
                                        cellRow.BorderWidth = 1.5F;

                                        if (j == gvRowFinal.Cells.Count -1)
                                        {
                                            cellRow.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                                        }
                                        pdfTable.AddCell(cellRow);
                                    }

                                    continue;
                                }

                                bool negrita = false;
                                DataGridViewRow gvRow = GrillaExportarPDF.Rows[i];
                                DataRow dRow = ventas.Rows[i];

                                try
                                {
                                    Convert.ToInt32(dRow[1].ToString());                                    
                                }
                                catch
                                { 
                                    negrita = true;
                                }

                                foreach (DataGridViewCell cell in gvRow.Cells)
                                {
                                    if(negrita)
                                    {
                                        PdfPCell cellRow = new PdfPCell(new Phrase(cell.Value.ToString(), FontFactory.GetFont(FontFactory.HELVETICA_BOLD)));
                                        cellRow.BorderWidth = 1.5F;
                                        pdfTable.AddCell(cellRow);
                                    }
                                    else
                                    {
                                        PdfPCell cellRow = new PdfPCell(new Phrase(cell.Value.ToString()));
                                        if(cell.ColumnIndex > 0)
                                        {
                                            cellRow.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                                        }                      
                                        pdfTable.AddCell(cellRow);
                                    }
                                }
                            }

                            using (FileStream stream = new FileStream(sfd.FileName, FileMode.Create))
                            {
                                Document pdfDoc = new Document(PageSize.A4, 10f, 20f, 20f, 10f);
                                PdfWriter.GetInstance(pdfDoc, stream);
                                pdfDoc.Open();
                                pdfDoc.Add(pdfTable);
                                pdfDoc.Close();
                                stream.Close();
                            }

                            MessageBox.Show("Descarga realizada.", "Info");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error :" + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("No se pudo exportar.");
            }
        }

        private void btnExportarEditar_Click(object sender, EventArgs e)
        {
            if(btnExportarEditar.Text == "Editar")
            {
                if(txtFechaExportarPDF.Text != txtFechaHastaExportarPDF.Text)
                {
                    MessageBox.Show("Las fechas deben coincidir.");
                    return;
                }

                btnFiltrarExportarPDF_Click(null, EventArgs.Empty);
                GrillaExportarPDF.Enabled = true;
                GrillaExportarPDF.ReadOnly = false;
                txtFechaExportarPDF.Enabled = false;
                txtFechaHastaExportarPDF.Enabled = false;
                btnExportarExportarPDF.Enabled = false;
                btnFiltrarExportarPDF.Enabled = false;
                btnExportarEditar.Text = "Aceptar";
                btnExportarCancelar.Visible = true;

                ventasViejas = new List<Ventas>();

                for (int i = 1; i < ventas.Rows.Count; i++)
                {
                    try
                    {
                        DataRow row = ventas.Rows[i];
                        Ventas v = new Ventas();

                        v.IdVenta = i;
                        v.Cantidad = Convert.ToInt32(row["Cantidad"].ToString());

                        ventasViejas.Add(v);
                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                GrillaExportarPDF.Enabled = false;
                GrillaExportarPDF.ReadOnly = true;
                txtFechaExportarPDF.Enabled = true;
                txtFechaHastaExportarPDF.Enabled = true;
                btnExportarExportarPDF.Enabled = true;
                btnFiltrarExportarPDF.Enabled = true;
                btnExportarEditar.Text = "Editar";
                btnExportarCancelar.Visible = false;

                fechaExportarFiltro = txtFechaExportarPDF.Value;

                foreach(Ventas venta in ventasModificadas)
                {
                    string pIdProducto = GenerarParametros("IdProducto", venta.IdProducto.ToString(), "int");
                    string pFecha = GenerarParametros("Fecha", fechaExportarFiltro.ToShortDateString(), "date");
                    string pCantidad = GenerarParametros("Cantidad", venta.Cantidad.ToString(), "int");
                    string sql = "EXEC VentasModificar " + pIdProducto + ", " + pFecha + ", " + pCantidad;
                    conexion.EjecutarQuery(sql);
                }

                ventasModificadas.Clear();
                btnFiltrarExportarPDF_Click(null, EventArgs.Empty);
            }

        }
        private void btnExportarCancelar_Click(object sender, EventArgs e)
        {
            GrillaExportarPDF.Enabled = false;
            GrillaExportarPDF.ReadOnly = true;
            txtFechaExportarPDF.Enabled = true;
            txtFechaHastaExportarPDF.Enabled = true;
            btnExportarExportarPDF.Enabled = true;
            btnFiltrarExportarPDF.Enabled = true;
            btnExportarEditar.Text = "Editar";
            btnExportarCancelar.Visible = false;

            ventasModificadas.Clear();
            btnFiltrarExportarPDF_Click(null, EventArgs.Empty);
        }

        private void GrillaExportarPDF_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            Productos p = ObtenerProducto("Producto", ventas.Rows[e.RowIndex]["Nombre"].ToString());
            try
            {
                if (!ventasModificadas.Exists(x => x.IdProducto == p.IdProducto))
                {
                    Ventas venta = new Ventas();
                    venta.IdProducto = p.IdProducto;
                    venta.Cantidad = Convert.ToInt32(ventas.Rows[e.RowIndex]["Cantidad"].ToString());

                    ventasModificadas.Add(venta);
                }
                else
                {
                    ventasModificadas.Find(x => x.IdProducto == p.IdProducto).Cantidad = Convert.ToInt32(ventas.Rows[e.RowIndex]["Cantidad"].ToString());
                }
            }
            catch
            {
                MessageBox.Show("La cantidad debe ser un numero.");
                GrillaExportarPDF.Rows[e.RowIndex].Cells["Cantidad"].Value = ventasViejas.Find(x => x.IdVenta == e.RowIndex).Cantidad;
            }
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
        private void AccionGrillaCategorias(object sender, DataGridViewCellEventArgs e)
        {
            Categorias c = new Categorias();
            c.IdCategoria = Convert.ToInt32(categoriasGrilla.Rows[e.RowIndex]["IdCategoria"].ToString());
            c.Categoria = categoriasGrilla.Rows[e.RowIndex]["Categoria"].ToString();
            c.Detalle = categoriasGrilla.Rows[e.RowIndex]["Detalle"].ToString();
            //p.IdEstado = productos.Rows[e.RowIndex]["Codigo"].ToString();

            if (categoriasGrilla.Columns[e.ColumnIndex].ColumnName == "Modificar")
            {
                if(c.IdCategoria == 1)
                {
                    MessageBox.Show("No se puede modificar la Categoria " +  c.Categoria +".");
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

                    string sql = "exec CategoriasBorrar " + pCategoria ;

                    conexion.EjecutarQuery(sql);
                    IniciarControlesCategorias();
                    IniciarControlesProductos();
                }
            }
        }
        private void AccionGrillaInicio(object sender, DataGridViewCellEventArgs e)
        {
            if (inicio.Columns[e.ColumnIndex].ColumnName == "Borrar")
            {
                BorrarVentas(e.RowIndex);
            }
        }

        
        private void PresionarTeclaPaginas(object sender, KeyPressEventArgs e)
        {
           
        }

        private void SeleccionarProductoInicio(object sender, EventArgs e)
        {
            foreach (DataRow row in productos.Rows)
            {
                if (row["Producto"].ToString() == ddlProductoInicio.SelectedText)
                {
                    Productos p = ObtenerProducto("Producto", row["Producto"].ToString());
                    AñadirVentas(p);
                    ddlProductoInicio.Focus();
                    break;
                }
            }
        }

        private void VaciarTextBox()
        {
            txtCodigoInicio.Text = "";
            ddlProductoInicio.Text = "";
        }

        private void AñadirVentas(Productos p)
        {
            try
            {
                bool existeProducto = productosVendidos.Exists(x => x.Producto == p.Producto);

                if (!existeProducto)
                {
                    inicio.Rows.Add(inicio.Rows.Count + 1, p.Producto, p.Categoria.Categoria, p.Precio, 1, "Borrar");
                    productosVendidos.Add(p);

                }
                else
                {
                    Productos productoExistente = productosVendidos.Find(x => x.Producto == p.Producto);

                    productoExistente.Cantidad++;

                    foreach(DataRow r in inicio.Rows)
                    {
                        if(r["Nombre"].ToString() == productoExistente.Producto)
                        {
                            r["Cantidad"] = productoExistente.Cantidad;
                            break;
                        }
                    }
                }

                CalcularTotal();
                VaciarTextBox();

                GrillaInicio.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                GrillaInicio.DataSource = inicio;
            }
            catch
            {
                MessageBox.Show("Error al añadir producto");
            }
        }
        private void InsertarVentas()
        {
            if (productosVendidos.Count != 0)
            {
                foreach (Productos p in productosVendidos)
                {
                    for(int i = 0; i<p.Cantidad; i++)
                    {
                        string pIdProducto = GenerarParametros("IdProducto", p.IdProducto.ToString(), "int");
                        string pFecha = GenerarParametros("Fecha", DateTime.Now.ToShortDateString(), "date");
                        string sql = "EXEC VentasInsertar " + pIdProducto + ", " + pFecha;
                        conexion.EjecutarQuery(sql);
                    }
                }

                VaciarTextBox();
                CalcularTotal();
                VaciarVentas();
                productosVendidos.Clear();
            }
            else
            {
                MessageBox.Show("No hay productos cargados");
            }

        }
        private void BorrarVentas(int index)
        {
            int contador = 0;

            foreach (Productos prod in productosVendidos)
            {
                if (contador == index)
                {
                    productosVendidos.Remove(prod);
                    break;
                }
                contador++;
            }
            inicio.Rows[index].Delete();
            CalcularTotal();
        }
        private void VaciarVentas()
        {
            productosVendidos.Clear();
            int cantidadRows = inicio.Rows.Count;
            for (int i = 1; i <= cantidadRows; i++)
            {
                inicio.Rows.Remove(inicio.Rows[0]);
            }
            CalcularTotal();
            VaciarTextBox();
        }

        private void CalcularTotal()
        {
            total = 0;
            foreach (Productos prod in productosVendidos)
            {
                for(int i = 0; i<prod.Cantidad; i++)
                {
                    total += prod.Precio;
                }
            }
            txtTotaIInicio.Text = total.ToString();
        }


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
            else if(tipo == "decimal")
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
            else if(tipo == "date")
            {
                string[] strs = valor.Split('/');
                if(strs[1].Length == 1)
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

                    foreach(DataRow row2 in categorias.Rows)
                    {
                        if(row2["IdCategoria"].ToString() == p.Categoria.IdCategoria.ToString())
                        {
                            p.Categoria.Categoria = row2["Categoria"].ToString();
                        }
                    }

                    break;
                }
            }
            return p;
        }

        private void PresionarTeclaPaginas(object sender, KeyEventArgs e)
        {
            if (Paginas.SelectedTab.Text == "Inicio")
            {
                if (e.KeyValue == (char)Keys.Enter)
                {
                    bool ingresarProducto = false;
                    
                    if(txtCodigoInicio.Text != string.Empty)
                    {
                        foreach (DataRow row in productos.Rows)
                        {
                            if (row["Codigo"].ToString() == txtCodigoInicio.Text)
                            {
                                Productos p = ObtenerProducto("Codigo", row["Codigo"].ToString());
                                AñadirVentas(p);
                                txtCodigoInicio.Focus();

                                ingresarProducto = true;
                                break;
                            }
                        }
                        if (!ingresarProducto)
                        {
                            VaciarTextBox();
                            MessageBox.Show("Error al ingresar producto.");
                        }
                    }
                }
                else if (e.KeyValue == (char)Keys.ControlKey)
                {
                    InsertarVentas();
                }
                else if (e.KeyValue == (char)Keys.Escape)
                {
                    VaciarVentas();
                }
            }
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

            int cantidadRows = ventas.Rows.Count;
            for (int i = 1; i <= cantidadRows; i++)
            {
                ventas.Rows.Remove(ventas.Rows[0]);
            }

        }

        private void GrillaInicio_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if(inicio.Columns[e.ColumnIndex].ColumnName == "Cantidad")
            {
                Productos p = productosVendidos.Find(x => x.Producto == inicio.Rows[e.RowIndex]["Nombre"].ToString());
                try
                {
                    p.Cantidad = Convert.ToInt32(inicio.Rows[e.RowIndex]["Cantidad"].ToString());
                    CalcularTotal();
                }
                catch 
                {
                    foreach (DataRow r in inicio.Rows)
                    {
                        if (r["Nombre"].ToString() == p.Producto)
                        {
                            r["Cantidad"] = p.Cantidad;
                            break;
                        }
                    }
                    MessageBox.Show("Error. Ingrese una cantidad valida.");
                }
            }
        }

        private void Mercante_Load(object sender, EventArgs e)
        {

        }
    }

}
