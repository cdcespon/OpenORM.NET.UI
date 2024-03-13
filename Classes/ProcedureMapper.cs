using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;


[Serializable]
public class MappedStoredProcedure
{
    private ObservableCollection<ResultSet> _resultSet = new ObservableCollection<ResultSet>();
    private ObservableCollection<ProcedureParameter> _procedureParameter = new ObservableCollection<ProcedureParameter>();
    private String _procedureName = String.Empty;

    [Serializable]
    public enum CustomTypeCode
    {
        Empty = 0,
        Object = 1,
        DBNull = 2,
        Boolean = 3,
        Char = 4,
        SByte = 5,
        Byte = 6,
        Int16 = 7,
        UInt16 = 8,
        Int32 = 9,
        UInt32 = 10,
        Int64 = 11,
        UInt64 = 12,
        Single = 13,
        Double = 14,
        Decimal = 15,
        DateTime = 16,
        String = 18,
        Guid = 19
    }


    public ObservableCollection<ResultSet> ResultSets
    {
        get { return _resultSet; }
        set { _resultSet = value; }
    }
    public ObservableCollection<ProcedureParameter> ProcedureParameters
    {
        get { return _procedureParameter; }
        set { _procedureParameter = value; }
    }

    public MappedStoredProcedure()
    {
        _resultSet.CollectionChanged += _resultSet_CollectionChanged;
        _procedureParameter.CollectionChanged += _procedureParameter_CollectionChanged;
        
    }

    void _procedureParameter_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        System.Diagnostics.Debug.Print(""); 
    }

    void _resultSet_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        System.Diagnostics.Debug.Print(""); 
    }
     [TypeConverter(typeof(SelectedProceduresConverter)), CategoryAttribute("Selected Procedures")]
    public String Name
    {
        get { return _procedureName; }
        set {
            _procedureName = value;
            OpenORM.UI.Classes.Global.UpdateProject();
        }
    }

    public MappedStoredProcedure(string name)
    {
        _procedureName = name;
    }

    [Serializable]
    public class ProcedureField
    {
        private CustomTypeCode _dataType = new CustomTypeCode();
        private String _name = String.Empty;
        private bool _isNullable = false;

        public ProcedureField()
        {
        }
        public ProcedureField(CustomTypeCode dataType, String name,bool isNullable)
        {
            DataType = dataType;
            Name = name;
            IsNullable = IsNullable;
        }

        public CustomTypeCode DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public bool IsNullable
        {
            get { return _isNullable; }
            set { _isNullable = value; }
        }

    }

    [Serializable]
    public class ResultSet
    {
        private ObservableCollection<ProcedureField> _procedureFields = new ObservableCollection<ProcedureField>();
        private String _name = String.Empty;

        public delegate void StatusUpdateHandler();
        public event StatusUpdateHandler OnUpdateStatus;


        public ResultSet()
        {
            _procedureFields.CollectionChanged += _procedureFields_CollectionChanged;
        }

        void _procedureFields_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
             
        }

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public ObservableCollection<ProcedureField> ProcedureFields
        {
            get { return _procedureFields; }
            set { _procedureFields = value; }
        }
    }

    [Serializable]
    public class ProcedureParameter
    {
        private CustomTypeCode _dataType = new CustomTypeCode();
        private String _name = String.Empty;

        public ProcedureParameter()
        {
        }
        public ProcedureParameter(CustomTypeCode dataType, String name)
        {
            DataType = dataType;
            Name = name;
        }

        public CustomTypeCode DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }


}

