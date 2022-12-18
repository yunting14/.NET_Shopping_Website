using System;
using System.Collections;

namespace Team2_DotNetCA.Models
{
    public class Purchase:IEnumerable
    { 

        public int PurchaseId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public IEnumerable<PurchasedProduct> p_list { get; set; }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)p_list).GetEnumerator();
        }
    }


    public class PurchasedProduct
    {
        public PurchasedProduct()
        {
            ACList = new List<Guid>();
        }

        public int ProductId { get; set; }
        public ICollection<Guid> ACList { get; set; }
        public string Image { get; set; }
        public string Details { get; set; }
        public string Name { get; set; }

        
    }

}

