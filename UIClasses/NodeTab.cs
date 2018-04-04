using EscapeDBUsage.Confirmations;
using EscapeDBUsage.InteractionRequests;
using EscapeDBUsage.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EscapeDBUsage.UIClasses
{
    public class NodeTab: NodeBase
    {
        public NodeTab(IEventAggregator eventAggregator, NodeExcel nodeExcel, MainViewModel viewModel) : base(eventAggregator)
        {
            this.viewModel = viewModel;
            NodeExcel = nodeExcel;
            AddTable = new DelegateCommand(DoAddTable);
            AddTables = new DelegateCommand(DoAddTables);

            AddTablesAndColumnsRequest = new AddTablesAndColumnsRequest();
        }

        private void DoAddTables()
        {
            AddTablesAndColumnsRequest.Raise(new AddTablesAndColumnsConfirmation(viewModel.DataViewModel) { Title = "Adding tables and columns..." }, (result) =>
            {
                if (result != null && result.Confirmed  && result.SelectedSprint != null && result.SelectedSprint.DbSchemaTables != null)
                {
                    foreach (var t in result.SelectedSprint.DbSchemaTables)
                    {
                        if (t.IsChecked && t.IsVisible)
                        {
                            var newDBTable = new NodeDbTable(EventAggregator, this, viewModel) { 
                                Name = t.Name
                            };
                            if (t.Columns != null)
                            {
                                newDBTable.Nodes = new ObservableCollection<NodeDbColumn>();
                                foreach (var c in t.Columns)
                                {
                                    if (c.IsChecked && c.IsVisible)
                                    {
                                        var newCol = new NodeDbColumn(EventAggregator, newDBTable, viewModel) { 
                                            Name = c.Name
                                        };
                                        newDBTable.Nodes.Add(newCol);
                                    }
                                }
                            }
                            this.Nodes.Add(newDBTable);
                        }
                    }
                }
            });           
        }

        private MainViewModel viewModel;

        public ICommand AddTable { get; private set; }
        public ICommand AddTables { get; private set; }

        private void DoAddTable()
        {
            if (Nodes == null) Nodes = new ObservableCollection<NodeDbTable>();
            var table = new NodeDbTable(EventAggregator, this, viewModel);
            Nodes.Insert(0, table);
            viewModel.SelectedDbTable = table;
            table.IsSelected = true;
        }

        private NodeExcel nodeExcel;
        public NodeExcel NodeExcel
        {
            get { return nodeExcel; }
            set { SetProperty(ref nodeExcel, value); }
        }

        private ObservableCollection<NodeDbTable> nodes;
        public ObservableCollection<NodeDbTable> Nodes
        {
            get { return nodes; }
            set { SetProperty(ref nodes, value); }
        }

        public AddTablesAndColumnsRequest AddTablesAndColumnsRequest { get; private set; }
    }
}
