namespace Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Codigo { get; set; } = string.Empty; 
        public string Nome { get; set; } = string.Empty; 
        public string Descricao { get; set; } = string.Empty; 
        public decimal Preco { get; set; }
        public bool Ativo { get; set; }
    }
}