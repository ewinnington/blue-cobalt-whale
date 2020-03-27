using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace using_pg_as_integration_test_database
{
    public class Asset
    {
        public string Name { get; set; }
        public List<Basin> Basins { get; set; }
        public List<Plant> Plants { get; set; }
    }

    public class Basin
    {
        public string Name { get; set; }
        public double Volume { get; set; }
        public double MinHeight { get; set; }
        public double MaxHeight { get; set; }
    }

    public class Plant
    {
        public string Name { get; set; }
        public List<Machine> Machines { get; set; }
    }

    public class Machine
    {
        public string Name { get; set; }
        public double Power { get; set; }
        public double Throughput { get; set; }
    }
}
