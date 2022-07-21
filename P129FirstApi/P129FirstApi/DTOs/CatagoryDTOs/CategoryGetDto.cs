using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P129FirstApi.DTOs.CatagoryDTOs
{
    public class CategoryGetDto
    {
        public int Id { get; set; }
        public string Ad { get; set; }
        public string Sekil { get; set; }
        public bool Esasdirmi { get; set; }
        public Nullable<int> AidOlduguUstCategoryId { get; set; }
    }
}
