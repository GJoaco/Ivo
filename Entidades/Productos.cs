using System;

namespace Entidades
{
    public class Productos
    {
        public int IdProducto { get; set; }
        public string Producto { get; set; }
        public decimal Precio { get; set; }
        public string Detalle { get; set; }
        public int IdEstado { get; set; }
        public string Codigo { get; set; }
        public int Cantidad { get; set; }
        public Categorias Categoria { get; set; }

        public Productos()
        {
            Categoria = new Categorias();
            Cantidad = 1;
        }
    }
}
