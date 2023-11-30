using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Windows.Controls;
using ScottPlot.Renderable;
using System.Windows;

namespace FFC_PDM
{
    // 콤보박스 그리고 데이터 펼치기
    class SearchDataFilterl : FacilityDataControl
    {
        public ComboBox MadeComboBox(ComboBox CB_ModelName)
        {
            // 구조 이해하기
            List<Machines> machinedata = GetMachinesData();
            // C# 리스트 구조 
            List<string> Id = new List<string>();
            foreach(Machines machine in machinedata)
            {
                // machine.model값이 없으면 Id 리스트에 넣는다.
               if (Id.Contains(machine.model) !=true)
                {
                    Id.Add(machine.model);
                }
            
            }
            //Id 정렬시키기
            Id.Sort();


            //ID 리스트 순회하는 코드 안에 콤보박스 아이템을 넣어야 한다. 
            foreach(string a in Id)
            {
                CB_ModelName.Items.Add(a);
            }
            return CB_ModelName;

            

        }
        // 콤보 박스 값 -> 집어넣고 -> 집어 넣은 값으로 데이터를 꺼낸다 
         public Dictionary<string, List<double>> MadeIDComboBox()
         {
            // 데이터 불러오기
            List<Machines> machines = GetMachinesData();
           
            Dictionary<string, List<double>> Id_model = new Dictionary<string, List<double>>();
            foreach (Machines machine in machines)
            {
                if (Id_model.ContainsKey(machine.model))
                {
                    Id_model[machine.model].Add(double.Parse(machine.machineID));
                }

                else
                {
                    Id_model[machine.model] = new List<double>();
                    Id_model[machine.model].Add(double.Parse(machine.machineID));
                }

            }

            foreach(string id in Id_model.Keys)
            {
                Id_model[id].Sort();
            }

            return Id_model;
         }

    }
}
