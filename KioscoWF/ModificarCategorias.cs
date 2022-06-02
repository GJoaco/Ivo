using Entidades;
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
    public partial class ModificarCategoria : Form
    {
        Conexion conexion;
        DataTable estados;
        public Categorias categorias;
        public ModificarCategoria(Categorias c)
        {
            categorias = c;
            InitializeComponent();

            txtNombreC.Text = categorias.Categoria;
            txtDetalleC.Text = categorias.Detalle;
        }
        private void Modificar_Categoria_Load(object sender, EventArgs e)
        {
            conexion = new Conexion();
            estados = conexion.ObtenerDataTable("exec EstadosSeleccionar");

         
            ddlEstadoC.DataSource = estados;
            ddlEstadoC.DisplayMember = "Estado";
            ddlEstadoC.ValueMember = "IdEstado";
        }
        private void btnModificarC_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(txtNombreC.Text))
            { 
                MessageBox.Show("Es obligatorio ingresar un nombre.");
                return;
            }
         

            categorias.Categoria = txtNombreC.Text;
            categorias.Detalle = txtDetalleC.Text;
            categorias.IdEstado = Convert.ToInt32(ddlEstadoC.SelectedValue);

            string pCategoria = GenerarParametros("Categoria", categorias.Categoria.ToString(), "string");
            string pIdCategoria = GenerarParametros("IdCategoria", categorias.IdCategoria.ToString(), "int");
            string pDetalle = GenerarParametros("Detalle", categorias.Detalle.ToString(), "string");
            string pEstado = GenerarParametros("IdEstado", categorias.IdEstado.ToString(), "int");

            string sql = "exec CategoriasModificar " + pIdCategoria + ", " + pCategoria + ", " + pDetalle + ", " + pEstado;

            conexion.EjecutarQuery(sql);
            Program.kiosco.IniciarControlesCategorias();
            Program.kiosco.IniciarControlesProductos();
            MessageBox.Show("La categoría se ha modificado con éxito.");
            this.Close();
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
