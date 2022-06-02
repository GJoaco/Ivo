using Entidades;
using KioscoWF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KioscoWF
{
    public partial class ModificarProductos : Form
    {       
        Conexion conexion;
        public Productos producto;
        public DataTable estados;
        public DataTable categorias;
        public DataTable productos;

        public ModificarProductos(Productos p)
        {
            InitializeComponent();
            conexion = new Conexion();

            categorias = conexion.ObtenerDataTable("exec CategoriasSeleccionar");
            estados = conexion.ObtenerDataTable("exec EstadosSeleccionar");

            ddlCategoria.DataSource = categorias;
            ddlCategoria.DisplayMember = "Categoria";
            ddlCategoria.ValueMember = "IdCategoria";

            ddlEstado.DataSource = estados;
            ddlEstado.DisplayMember = "Estado";
            ddlEstado.ValueMember = "IdEstado";
            
            producto = p;

            txtNombre.Text = producto.Producto;
            txtPrecio.Text = producto.Precio.ToString();
            txtDetalle.Text = producto.Detalle;
            txtCodigo.Text = producto.Codigo;
            ddlCategoria.SelectedValue = producto.Categoria.IdCategoria;
            ddlEstado.SelectedIndex = producto.IdEstado;

            ddlEstado.SelectedValue = producto.IdEstado.ToString();
            ddlCategoria.SelectedValue = producto.Categoria.IdCategoria.ToString();
        }

        private void ModificarProductos_Load(object sender, EventArgs e)
        {
            productos = conexion.ObtenerDataTable("exec ProductosSeleccionar");
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            try
            {
                if(string.IsNullOrEmpty(txtNombre.Text))
                { 
                    MessageBox.Show("Es obligatorio ingresar un nombre.");
                    return; 
                }

                producto.Producto = txtNombre.Text;
                producto.Detalle = txtDetalle.Text;
                producto.Codigo = txtCodigo.Text;
                bool ExisteCodigo = false;

                if (txtPrecio.Text.Contains("."))
                {
                    producto.Precio = Convert.ToDecimal(txtPrecio.Text.Replace(".", ","));
                }
                else
                {
                    producto.Precio = Convert.ToDecimal(txtPrecio.Text);
                }

                producto.Categoria.IdCategoria = ddlCategoria.SelectedValue.ToString() == string.Empty ? 0 : Convert.ToInt32(ddlCategoria.SelectedValue);
                producto.IdEstado = ddlEstado.SelectedValue.ToString() == string.Empty ? 0 : Convert.ToInt32(ddlEstado.SelectedValue);
                Int32.TryParse(txtStock.Text, out int stock);
                producto.Stock = stock;

                foreach (DataRow row in productos.Rows)
                {
                    if (row["Codigo"].ToString() != "")
                    {
                        if ((producto.Codigo == row["Codigo"].ToString()) && (producto.IdProducto.ToString() != row["IdProducto"].ToString()))
                        {
                            ExisteCodigo = true;
                            break;
                        }
                    }
                }

                if (!ExisteCodigo)
                {
                    string pIdProducto = GenerarParametros("IdProducto", producto.IdProducto.ToString(), "int"); //Aca va IdProducto
                    string pProducto = GenerarParametros("Producto", producto.Producto, "string"); //Aca va IdProducto
                    string pPrecio = GenerarParametros("Precio", producto.Precio.ToString(), "decimal"); //Aca va IdProducto
                    string pCategoria = GenerarParametros("IdCategoria", producto.Categoria.IdCategoria.ToString(), "int"); //Aca va IdProducto
                    string pDetalle = GenerarParametros("Detalle", producto.Detalle, "string"); //Aca va IdProducto
                    string pCodigo = GenerarParametros("Codigo", producto.Codigo, "string"); //Aca va IdProducto
                    string pStock = GenerarParametros("Stock", producto.Stock.ToString(), "int"); //Aca va IdProducto
                    string pEstado = GenerarParametros("IdEstado", producto.IdEstado.ToString(), "int"); //Aca va IdProducto

                    string sql = "exec ProductosModificar " + pIdProducto + ',' + pProducto + ',' + pPrecio + ',' + pCategoria + ',' + pDetalle + ',' + pCodigo + ',' + pEstado + ',' + pStock;

                    conexion.EjecutarQuery(sql);
                    Program.kiosco.ddlCategoriaProductosListar.SelectedValue = 0;
                    Program.kiosco.IniciarControlesProductos();
                    MessageBox.Show("el producto se ha modificado con éxito.");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Ya existe un producto con ese código.");
                }
            }
            catch
            {
                MessageBox.Show("Error al modificar producto");
            }
        }



        private string GenerarParametros(string nombre,string valor, string tipo)
        {
            string parametros = "@";

            if(tipo == "string")
            {
                parametros += nombre + "='" + valor + "' ";
            }
            else if(tipo == "int")
            {
                parametros += nombre + "=" + valor + " ";
            }
            else
            {
                if(valor.Contains(","))
                {
                    parametros += nombre + "=" + valor.Replace(",",".") + " ";
                }
            }
            return parametros;
        }

  
    }
}
