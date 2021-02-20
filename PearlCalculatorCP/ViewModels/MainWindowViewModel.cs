using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PearlCalculatorCP.Models;
using PearlCalculatorLib.General;
using PearlCalculatorLib.PearlCalculationLib.Entity;
using PearlCalculatorLib.PearlCalculationLib.World;
using PearlCalculatorLib.Result;
using ReactiveUI;

using ManuallyCalculation = PearlCalculatorLib.Manually.Calculation;
using ManuallyData = PearlCalculatorLib.Manually.Data;

namespace PearlCalculatorCP.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        /// <summary>
        /// On pearl offset x value changed
        /// <param name="parameter1">last value</param>
        /// <param name="parameter2">next value</param>
        /// <param name="parameter3">supress callback</param>
        /// <param name="Parameter4">backing field value</param>
        /// </summary>
        /// <returns>bool: can change this value; double: target value</returns>
        public event Func<string, string, Action, double, (bool, double)>? OnPearlOffsetXTextChanged;
        
        /// <summary>
        /// On pearl offset z value changed
        /// <param name="parameter1">last value</param>
        /// <param name="parameter2">next value</param>
        /// <param name="parameter3">supress callback</param>
        /// <param name="Parameter4">backing field value</param>
        /// </summary>
        /// <returns>bool: can change this value; double: target value</returns>
        public event Func<string, string, Action, double, (bool, double)>? OnPearlOffsetZTextChanged;

        private bool _isSupressX = false;
        private bool _isSupressZ = false;

        public static int MaxTicks { get; set; } = 100;

        #region GeneralFTL General Input Data
        
        private double _pearlPosX;
        public double PearlPosX
        {
            get => _pearlPosX;
            set
            {
                this.RaiseAndSetIfChanged(ref _pearlPosX, value);
                if (_pearlPosX != Data.Pearl.Position.X)
                    Data.Pearl.Position.X = _pearlPosX;
            }
        }

        private double _pearlPosZ;
        public double PearlPosZ
        {
            get => _pearlPosZ;
            set
            {
                this.RaiseAndSetIfChanged(ref _pearlPosZ, value);
                if (_pearlPosZ != Data.Pearl.Position.Z)
                    Data.Pearl.Position.Z = _pearlPosZ;
            }
        }

        private double _destinationX;
        public double DestinationX
        {
            get => _destinationX;
            set
            {
                this.RaiseAndSetIfChanged(ref _destinationX, value);
                if (_destinationX != Data.Destination.X)
                    Data.Destination.X = _destinationX;
            }
        }

        private double _destinationZ;
        public double DestinationZ
        {
            get => _destinationZ;
            set
            {
                this.RaiseAndSetIfChanged(ref _destinationZ, value);
                if (_destinationZ != Data.Destination.Z)
                    Data.Destination.Z = _destinationZ;
            }
        }

        public uint MaxTNT
        {
            get => (uint)Data.MaxTNT;
            set => this.RaiseAndSetIfChanged(ref Data.MaxTNT, (int)value);
        }

        public Direction Direction
        {
            get => Data.Direction;
            set => this.RaiseAndSetIfChanged(ref Data.Direction, value);
        }

        public uint RedTNT
        {
            get => (uint)Data.RedTNT;
            set => this.RaiseAndSetIfChanged(ref Data.RedTNT, (int)value);
        }

        public uint BlueTNT
        {
            get => (uint)Data.BlueTNT;
            set => this.RaiseAndSetIfChanged(ref Data.BlueTNT, (int)value);
        }

        #endregion

        #region GeneralFTL Advanced Input Data

        private string _pearlOffsetX = "0.";
        public string PearlOffsetX
        {
            get => _pearlOffsetX;
            set
            {
                (bool, double) callbackResult;
                if (!_isSupressX && OnPearlOffsetXTextChanged != null &&
                    (callbackResult = OnPearlOffsetXTextChanged.Invoke(_pearlOffsetX, value, () => _isSupressX = true, Data.PearlOffset.Z)).Item1)
                {
                    this.RaiseAndSetIfChanged(ref _pearlOffsetX, value);
                    Data.PearlOffset.X = callbackResult.Item2;
                }
                else _isSupressX = false;
            }
        }

        private string _pearlOffsetZ = "0.";
        public string PearlOffsetZ
        {
            get => _pearlOffsetZ;
            set
            {
                (bool, double) callbackResult;
                if (!_isSupressZ && OnPearlOffsetZTextChanged != null &&
                    (callbackResult = OnPearlOffsetZTextChanged.Invoke(_pearlOffsetZ, value, () => _isSupressZ = true, Data.PearlOffset.Z)).Item1)
                {
                    this.RaiseAndSetIfChanged(ref _pearlOffsetZ, value);
                    Data.PearlOffset.Z = callbackResult.Item2;
                }
                else _isSupressZ = false;
            }
        }
        
        public int TNTWeight
        {
            get => Data.TNTWeight;
            set => this.RaiseAndSetIfChanged(ref Data.TNTWeight, value);
        }
        
        private TNTWeightModeEnum _tntWeight;
        public TNTWeightModeEnum TNTWeightMode
        {
            get => _tntWeight;
            set => this.RaiseAndSetIfChanged(ref _tntWeight, value);
        }
        
        #endregion

        #region GeneralFTL Result Data

        #region Calculation TNT Amount

        private ObservableCollection<TNTCalculationResult>? _tntResult;
        public ObservableCollection<TNTCalculationResult>? TNTResult
        {
            get => _tntResult;
            set
            {
                _tntResult = value;
                this.RaisePropertyChanged();
            }
        }

        private int _tntResultSelectedIndex = -1;
        public int TNTResultSelectedIndex
        {
            get => _tntResultSelectedIndex;
            set
            {
                if (value >= 0 && value < TNTResult.Count)
                {
                    this.RaiseAndSetIfChanged(ref _tntResultSelectedIndex, value);
                    Direction = Data.Pearl.Position.Direction(Data.Pearl.Position.WorldAngle(Data.Destination));
                    BlueTNT = (uint)_tntResult[value].Blue;
                    RedTNT = (uint) _tntResult[value].Red;
                }
            }
        }
        
        #endregion

        #region Pearl trace

        private List<PearlTraceModel>? _pearlTraceList;
        public List<PearlTraceModel>? PearlTraceList
        {
            get => _pearlTraceList;
            set => this.RaiseAndSetIfChanged(ref _pearlTraceList, value);
        }
        
        #endregion

        private string _resultDirection = string.Empty;
        public string ResultDirection
        {
            get => _resultDirection;
            set => this.RaiseAndSetIfChanged(ref _resultDirection, value);
        }

        private string _resultAngle = string.Empty;
        public string ResultAngle
        {
            get => _resultAngle;
            set => this.RaiseAndSetIfChanged(ref _resultAngle, value);
        }

        private bool _isDisplayTNTAmount = true;
        public bool IsDisplayTNTAmount
        {
            get => _isDisplayTNTAmount;
            set => this.RaiseAndSetIfChanged(ref _isDisplayTNTAmount, value);
        }

        private bool _isDisplayMotion = false;
        public bool IsDisplayMotion
        {
            get => _isDisplayMotion;
            set => this.RaiseAndSetIfChanged(ref _isDisplayMotion, value);
        }

        #endregion

        #region Console
        
        private string _commandText;
        public string CommandText
        {
            get => _commandText;
            set => this.RaiseAndSetIfChanged(ref _commandText, value);
        }

        private ObservableCollection<ConsoleOutputItemModel>? _consoleOutputs;
        public ObservableCollection<ConsoleOutputItemModel>? ConsoleOutputs
        {
            get => _consoleOutputs;
            set => this.RaiseAndSetIfChanged(ref _consoleOutputs, value);
        }

        #endregion

        public MainWindowViewModel()
        {
            ConsoleOutputs ??= new ObservableCollection<ConsoleOutputItemModel>();
            
            CommandManager.Instance.OnMessageSend += AddConsoleMessage;
        }
        
        ~MainWindowViewModel()
        {
            CommandManager.Instance.OnMessageSend -= AddConsoleMessage;
        }

        private void AddConsoleMessage(ConsoleOutputItemModel message)
        {
            if(ConsoleOutputs.Count >= 500)
                for (int i = 0; i < 50; i++)
                    ConsoleOutputs.RemoveAt(0);
            
            ConsoleOutputs.Add(message);
        }

        public void LoadDataFormSettings(Settings settings)
        {
            Data.NorthWestTNT = settings.NorthWestTNT;
            Data.NorthEastTNT = settings.NorthEastTNT;
            Data.SouthWestTNT = settings.SouthWestTNT;
            Data.SouthEastTNT = settings.SouthEastTNT;
            
            Data.Pearl = settings.Pearl;
            
            Data.Destination = settings.Destination;
            
            PearlPosX = settings.Pearl.Position.X;
            PearlPosZ = settings.Pearl.Position.Z;
            DestinationX = settings.Destination.X;
            DestinationZ = settings.Destination.Z;
            PearlOffsetX = settings.Offset.X.ToString();
            PearlOffsetZ = settings.Offset.Z.ToString();
            BlueTNT = (uint)settings.BlueTNT;
            RedTNT = (uint) settings.RedTNT;
            MaxTNT = (uint)settings.MaxTNT;
            Direction = settings.Direction;
        }

        #region Calculate
        
        public void CalculateTNTAmount()
        {
            if (Calculation.CalculateTNTAmount(MaxTicks, 10))
            {
                SortTNTResult();
                ShowTNTAmount(Data.TNTResult);
            }
            IsDisplayTNTAmount = true;
        }

        public void PearlSimulate()
        {
            ShowPearlTrace(Calculation.CalculatePearlTrace((int)RedTNT, (int)BlueTNT, MaxTicks, Direction));
        }
        
        #endregion
        
        #region TNT Result Sort

        private void SortTNTResult()
        {
            switch (TNTWeightMode)
            {
                case TNTWeightModeEnum.DistanceVSTNT:
                    Data.TNTResult.SortByWeightedDistance(new TNTResultSortByWeightedArgs(TNTWeight, Data.MaxCalculateTNT, Data.MaxCalculateDistance));
                    break;
                case TNTWeightModeEnum.OnlyTNT:
                    Data.TNTResult.SortByWeightedTotal(new TNTResultSortByWeightedArgs(TNTWeight, Data.MaxCalculateTNT, Data.MaxCalculateDistance));
                    break;
                case TNTWeightModeEnum.OnlyDistance:
                    Data.TNTResult.SortByDistance();
                    break;
            }
        }

        public void SortTNTResultByDistance()
        {
            Data.TNTResult.SortByDistance();
            TNTResult = new ObservableCollection<TNTCalculationResult>(Data.TNTResult);
        }

        public void SortTNTResultByTicks()
        {
            Data.TNTResult.SortByTick();
            TNTResult = new ObservableCollection<TNTCalculationResult>(Data.TNTResult);
        }

        public void SortTNTResultByTotal()
        {
            Data.TNTResult.SortByTotal();
            TNTResult = new ObservableCollection<TNTCalculationResult>(Data.TNTResult);
        }
        
        #endregion

        #region Maunally Calcualte

        public void ManuallyCalculateTNTAmount()
        {
            if (ManuallyCalculation.CalculateTNTAmount(ManuallyData.Destination, MaxTicks, out var result))
                ShowTNTAmount(result);
            IsDisplayTNTAmount = true;
        }

        public void ManuallyCalculatePearlTrace()
        {
            ShowPearlTrace(ManuallyCalculation.CalculatePearl(ManuallyData.ATNTAmount, ManuallyData.BTNTAmount, MaxTicks));
            
            ResultDirection = string.Empty;
            ResultAngle = string.Empty;
        }

        public void ManuallyCalculatePearlMomentum()
        {
            var entities = ManuallyCalculation.CalculatePearl(ManuallyData.ATNTAmount, ManuallyData.BTNTAmount, MaxTicks);
            var traces = new List<PearlTraceModel>(entities.Count);
            traces.AddRange(entities.Select((t, i) => new PearlTraceModel {Tick = i, XCoor = t.Motion.X, YCoor = t.Motion.Y, ZCoor = t.Motion.Z}));
            PearlTraceList = traces;

            IsDisplayTNTAmount = false;
            IsDisplayMotion = true;
            ResultDirection = string.Empty;
            ResultAngle = string.Empty;

        }

        #endregion

        #region Result Show
        
        private void ShowPearlTrace(List<Entity> entities)
        {
            var traces = new List<PearlTraceModel>(entities.Count);
            traces.AddRange(entities.Select((t, i) => new PearlTraceModel {Tick = i, XCoor = t.Position.X, YCoor = t.Position.Y, ZCoor = t.Position.Z}));
            PearlTraceList = traces;

            IsDisplayTNTAmount = false;
            IsDisplayMotion = false;
        }

        private void ShowTNTAmount(List<TNTCalculationResult> result)
        {
            TNTResult = new ObservableCollection<TNTCalculationResult>(result);
            var angle = ManuallyData.Pearl.Position.WorldAngle(ManuallyData.Destination);
            ResultDirection = ManuallyData.Pearl.Position.Direction(angle).ToString();
            ResultAngle = angle.ToString();
        }
        
        #endregion
        
        public void SendCmd()
        {
            if(string.IsNullOrEmpty(CommandText) || string.IsNullOrWhiteSpace(CommandText) || CommandText[0] != '/')
                return;

            var cmd = CommandText[1..];
            CommandText = string.Empty;
            
            CommandManager.Instance.ExcuteCommand(cmd);
        }

    }
    
    public enum TNTWeightModeEnum
    {
        DistanceVSTNT, OnlyTNT, OnlyDistance
    }
}