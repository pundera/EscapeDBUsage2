using EscapeDBUsage.Classes;
using EscapeDBUsage.Events;
using EscapeDBUsage.UIClasses;
using EscapeDBUsage.UIClasses.DatabaseSchema;
using EscapeDBUsage.UIControlGetters;
using EscapeDBUsage.Views;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace EscapeDBUsage.ViewModels
{
    public class DatabaseSchemaViewModel: BindableBase
    {
        public DatabaseSchemaViewModel(IEventAggregator evAgg)
        {
            this.eventAgg = evAgg;
            eventAgg.GetEvent<SelectedSprintChanged>().Subscribe((sprint) => {
                SelectedSprint = sprint;
                //ExpandAll.Execute(null);
            });


            UncheckAll = new DelegateCommand(() =>
            {
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    foreach (var t in SelectedSprint.DbSchemaTables)
                    {
                        var tableUncheck = new Action(() =>
                        {
                            t.IsChecked = false;
                            if (t.Columns != null)
                            {
                                foreach (var c in t.Columns)
                                {
                                    c.IsChecked = false;
                                }
                            }
                        });
                        Dispatcher.CurrentDispatcher.Invoke(tableUncheck, DispatcherPriority.ApplicationIdle, null);
                    }
                });
            });
            
            CheckAll = new DelegateCommand(() =>
            {
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    foreach (var t in SelectedSprint.DbSchemaTables)
                    {
                        var tableCheck = new Action(() =>
                        {
                            t.IsChecked = true;
                            if (t.Columns != null)
                            {
                                foreach (var c in t.Columns)
                                {
                                    c.IsChecked = true;
                                }
                            }
                        });
                        Dispatcher.CurrentDispatcher.Invoke(tableCheck, DispatcherPriority.ApplicationIdle, null);
                    }
                });
            });

            CollapseAll = new DelegateCommand(() => {
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    foreach (var t in SelectedSprint.DbSchemaTables)
                    {
                        var tableCollapse = new Action(() =>
                        {
                            t.IsExpanded = false;
                        });
                        Dispatcher.CurrentDispatcher.Invoke(tableCollapse, DispatcherPriority.ApplicationIdle, null);
                    }
                });
            });

            Action expandAllAction = async () =>
            {
                Dispatcher.CurrentDispatcher.Invoke(new Action (() => {
                    //IsDbSchemaTabVisible = false;
                }), DispatcherPriority.Normal, null);

                await Task.Run(() =>
                {
                    foreach (var t in SelectedSprint.DbSchemaTables)
                    {
                        Action tableExpand = () =>
                        {
                            t.IsExpanded = true;
                        };

                        Dispatcher.CurrentDispatcher.Invoke(tableExpand, DispatcherPriority.Normal, null);
                    }
                });
                //.ContinueWith(new Action<Task>((t) => {
                   
                //}));

                Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
                {
                    //IsDbSchemaTabVisible = true;
                }), DispatcherPriority.Normal, null);

            };

            ExpandAll = new DelegateCommand(() =>
            {
                //VisualChildrenHelper.FreezeAll(VisualChildrenHelper.FindVisualChild<DatabaseSchemaView>(Application.Current.MainWindow));
                expandAllAction();                
            });

            EraseSchemaFulltext = new DelegateCommand(() => {
                SchemaTableFulltext = null;
                SchemaColumnFulltext = null;
            });
        }

        private bool isDbSchemaTabVisible = true;
        public bool IsDbSchemaTabVisible
        {
            get { return isDbSchemaTabVisible; }
            set
            {
                SetProperty(ref isDbSchemaTabVisible, value);
            }
        }

        private IEventAggregator eventAgg;

        private string schemaColumnFulltext;
        public string SchemaColumnFulltext
        {
            get { return schemaColumnFulltext; }
            set { 
                SetProperty(ref schemaColumnFulltext, value);
            
                if (value == null)
                {
                    EraseFulltext(SchemaFultextType.Column);
                }
                else
                {
                    SchemaTableFulltext = null;
                    DoFulltext(SchemaFultextType.Column);
                }
            }
        }

        private UISprint selectedSprint;
        public UISprint SelectedSprint
        {
            get { return selectedSprint; }
            set
            {
                SetProperty(ref selectedSprint, value);
            }
        }


        public ICommand CheckAll { get; private set; }
        public ICommand UncheckAll { get; private set; }

        public ICommand CollapseAll { get; private set; }
        public ICommand ExpandAll { get; private set; }

        private void DoFulltext(SchemaFultextType schemaFultextType)
        {
            foreach (var t in SelectedSprint.DbSchemaTables)
            {
                if (schemaFultextType == SchemaFultextType.Table)
                {
                    if (t.Name.ToUpperInvariant().Contains(SchemaTableFulltext.ToUpperInvariant())) t.IsVisible = true;
                    else t.IsVisible = false;
                }

                if (schemaFultextType == SchemaFultextType.Column)
                {
                    if (t.Columns != null)
                    {
                        var b = false;
                        foreach (var c in t.Columns)
                        {
                            if (c.Name.ToUpperInvariant().Contains(SchemaColumnFulltext.ToUpperInvariant()))
                            {
                                c.IsVisible = true;
                                t.IsVisible = true;
                                b = true;
                            }
                            else
                            {

                                c.IsVisible = false;
                                //t.IsVisible = false;
                            }
                        }
                        t.IsVisible = b;
                    }
                }
            }
        }

        private void EraseFulltext(SchemaFultextType schemaFultextType)
        {
            foreach (var t in SelectedSprint.DbSchemaTables)
            {
                t.IsVisible = true;
                if (t.Columns != null) 
                    foreach (var c in t.Columns)
                    {
                        c.IsVisible = true;
                    }
            }
        }

        private string schemaTableFulltext;
        public string SchemaTableFulltext
        {
            get { return schemaTableFulltext; }
            set
            {
                SetProperty(ref schemaTableFulltext, value);

                if (value == null)
                {
                    EraseFulltext(SchemaFultextType.Table);
                }
                else
                {
                    SchemaColumnFulltext = null;
                    DoFulltext(SchemaFultextType.Table);
                }
            }
        }
        public ICommand EraseSchemaFulltext { get; private set; }

    }

    public enum SchemaFultextType
    {
        Table,
        Column
    }
}
