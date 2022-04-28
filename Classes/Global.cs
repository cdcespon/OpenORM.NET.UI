using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenORM.UI.Classes
{
    public static class Global
    {
        static GenerationProject _currentProject = new GenerationProject();
        public static GenerationProject CurrentProject
        {
            get
            {
                return _currentProject;

            }
            set {
                _currentProject = value;
            }
 
        }
        public static void UpdateProject()
        {
            GenerationProjectSerializer.Save(_currentProject); 
        }
    }
}
