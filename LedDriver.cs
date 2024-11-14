using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace WakeUpAPI.Gpio.LED
{
    public class LedDriver
    {
        private static int _pixelCount = 0;
        private static bool _initialized;
        private static ISpiChannel _spiChannel;
        private static LedStripWs2801 _ledStrip;
        private static CancellationTokenSource _taskCancellationTokenSource;
        private static CancellationTokenSource _ledCancellationTokenSource;
        private static Task _currentRunningTask;
        private static bool _recurringTaskCancelled = false;
        public static bool ProgramRunning { get; internal set; } = false;

        public static void Initialize(int pixelCount)
        {
            if (_initialized) return;

            _pixelCount = pixelCount;
            Pi.Init<BootstrapWiringPi>();
            
            Pi.Spi.Channel0Frequency = LedStripWs2801.Ws2801SpiChannelFrequency;
            _spiChannel = Pi.Spi.Channel0;
            _ledStrip = new LedStripWs2801(_pixelCount, _spiChannel);
            _initialized = true;
        }

        public static async Task RunProgramOnce(Action<LedStripWs2801, CancellationToken> onAction)
        {
            if (!_initialized)
            {
                Trace.WriteLine("LedDriver not initialized! Invoke 'LedDriver.Initialize()' first!");
                return;
            }

            if (ProgramRunning)
            {
                Trace.WriteLine("Currently a program is running! Invoke 'LedDriver.StopRunningProgram()' first!");
                return;
            }

            using (_taskCancellationTokenSource = new CancellationTokenSource())
            {
                ProgramRunning = true;
                _currentRunningTask = Task.Run(() =>
                {
                    using (_ledCancellationTokenSource = new CancellationTokenSource())
                    {
                        onAction(_ledStrip, _ledCancellationTokenSource.Token);
                    }
                }, _taskCancellationTokenSource.Token);

                if (_currentRunningTask.Status == TaskStatus.Running
                   || _currentRunningTask.Status == TaskStatus.Created
                   || _currentRunningTask.Status == TaskStatus.WaitingToRun
                   || _currentRunningTask.Status == TaskStatus.WaitingForActivation
                   || _currentRunningTask.Status == TaskStatus.WaitingForChildrenToComplete)
                    await _currentRunningTask;
                ProgramRunning = false;
            }
        }

        public static async Task RunProgramOnce(Action<LedStripWs2801, CancellationToken, int> onAction, int param1)
        {
            if (!_initialized)
            {
                Trace.WriteLine("LedDriver not initialized! Invoke 'LedDriver.Initialize()' first!");
                return;
            }

            if (ProgramRunning)
            {
                Trace.WriteLine("Currently a program is running! Invoke 'LedDriver.StopRunningProgram()' first!");
                return;
            }

            using (_taskCancellationTokenSource = new CancellationTokenSource())
            {
                ProgramRunning = true;
                _currentRunningTask = Task.Run(() =>
               {
                   using (_ledCancellationTokenSource = new CancellationTokenSource())
                   {
                       onAction(_ledStrip, _ledCancellationTokenSource.Token, param1);
                   }
               }, _taskCancellationTokenSource.Token);

                if (_currentRunningTask.Status == TaskStatus.Running
                   || _currentRunningTask.Status == TaskStatus.Created
                   || _currentRunningTask.Status == TaskStatus.WaitingToRun
                   || _currentRunningTask.Status == TaskStatus.WaitingForActivation
                   || _currentRunningTask.Status == TaskStatus.WaitingForChildrenToComplete)
                    await _currentRunningTask;
                ProgramRunning = false;
            }
        }

        public static async Task RunProgramOnce(Action<LedStripWs2801, CancellationToken, int, int> onAction, int param1 = 0, int param2 = 0)
        {
            if (!_initialized)
            {
                Trace.WriteLine("LedDriver not initialized! Invoke 'LedDriver.Initialize()' first!");
                return;
            }

            if (ProgramRunning)
            {
                Trace.WriteLine("Currently a program is running! Invoke 'LedDriver.StopRunningProgram()' first!");
                return;
            }

            using (_taskCancellationTokenSource = new CancellationTokenSource())
            {
                ProgramRunning = true;
                _currentRunningTask = Task.Run(() =>
                {
                    using (_ledCancellationTokenSource = new CancellationTokenSource())
                    {
                        onAction(_ledStrip, _taskCancellationTokenSource.Token, param1, param2);
                    }
                }, _taskCancellationTokenSource.Token);

                if (_currentRunningTask.Status == TaskStatus.Running
                   || _currentRunningTask.Status == TaskStatus.Created
                   || _currentRunningTask.Status == TaskStatus.WaitingToRun
                   || _currentRunningTask.Status == TaskStatus.WaitingForActivation
                   || _currentRunningTask.Status == TaskStatus.WaitingForChildrenToComplete)
                    await _currentRunningTask;
                ProgramRunning = false;
            }
        }

        public static async Task RunProgramOnce(Action<LedStripWs2801, CancellationToken, int, int, int> onAction, int param1 = 0, int param2 = 0, int param3 = 0)
        {
            if (!_initialized)
            {
                Trace.WriteLine("LedDriver not initialized! Invoke 'LedDriver.Initialize()' first!");
                return;
            }

            if (ProgramRunning)
            {
                Trace.WriteLine("Currently a program is running! Invoke 'LedDriver.StopRunningProgram()' first!");
                return;
            }

            using (_taskCancellationTokenSource = new CancellationTokenSource())
            {
                ProgramRunning = true;
                _currentRunningTask = Task.Run(() =>
                {
                    using (_ledCancellationTokenSource = new CancellationTokenSource())
                    {
                        onAction(_ledStrip, _taskCancellationTokenSource.Token, param1, param2, param3);
                    }
                }, _taskCancellationTokenSource.Token);

                if (_currentRunningTask.Status == TaskStatus.Running
                   || _currentRunningTask.Status == TaskStatus.Created
                   || _currentRunningTask.Status == TaskStatus.WaitingToRun
                   || _currentRunningTask.Status == TaskStatus.WaitingForActivation
                   || _currentRunningTask.Status == TaskStatus.WaitingForChildrenToComplete)
                    await _currentRunningTask;
                ProgramRunning = false;
            }
        }

        public static async Task RunProgramRecurring(Action<LedStripWs2801, CancellationToken> onAction)
        {
            if (ProgramRunning)
            {
                Trace.WriteLine("Currently a program is running! Invoke 'LedDriver.StopRunningProgram()' first!");
                return;
            }

            while (!_recurringTaskCancelled)
            {
                _currentRunningTask = RunProgramOnce(onAction);

                if (_currentRunningTask.Status == TaskStatus.Running
                   || _currentRunningTask.Status == TaskStatus.Created
                   || _currentRunningTask.Status == TaskStatus.WaitingToRun
                   || _currentRunningTask.Status == TaskStatus.WaitingForActivation
                   || _currentRunningTask.Status == TaskStatus.WaitingForChildrenToComplete)
                    await _currentRunningTask;
            }

            _recurringTaskCancelled = false;
        }

        public static async Task RunProgramRecurring(Action<LedStripWs2801, CancellationToken, int> onAction, int param1 = 0)
        {
            if (ProgramRunning)
            {
                Trace.WriteLine("Currently a program is running! Invoke 'LedDriver.StopRunningProgram()' first!");
                return;
            }

            while (!_recurringTaskCancelled)
            {
                _currentRunningTask = RunProgramOnce(onAction, param1);

                if (_currentRunningTask.Status == TaskStatus.Running
                   || _currentRunningTask.Status == TaskStatus.Created
                   || _currentRunningTask.Status == TaskStatus.WaitingToRun
                   || _currentRunningTask.Status == TaskStatus.WaitingForActivation
                   || _currentRunningTask.Status == TaskStatus.WaitingForChildrenToComplete)
                    await _currentRunningTask;
            }

            _recurringTaskCancelled = false;
        }

        public static async Task RunProgramRecurring(Action<LedStripWs2801, CancellationToken, int, int> onAction, int param1 = 0, int param2 = 0)
        {
            if (ProgramRunning)
            {
                Trace.WriteLine("Currently a program is running! Invoke 'LedDriver.StopRunningProgram()' first!");
                return;
            }

            while (!_recurringTaskCancelled)
            {
                _currentRunningTask = RunProgramOnce(onAction, param1, param2);

                if (_currentRunningTask.Status == TaskStatus.Running
                   || _currentRunningTask.Status == TaskStatus.Created
                   || _currentRunningTask.Status == TaskStatus.WaitingToRun
                   || _currentRunningTask.Status == TaskStatus.WaitingForActivation
                   || _currentRunningTask.Status == TaskStatus.WaitingForChildrenToComplete)
                    await _currentRunningTask;
            }

            _recurringTaskCancelled = false;
        }

        public static async Task RunProgramRecurring(Action<LedStripWs2801, CancellationToken, int, int, int> onAction, int param1 = 0, int param2 = 0, int param3 = 0)
        {
            if (ProgramRunning)
            {
                Trace.WriteLine("Currently a program is running! Invoke 'LedDriver.StopRunningProgram()' first!");
                return;
            }

            while (!_recurringTaskCancelled)
            {
                _currentRunningTask = RunProgramOnce(onAction, param1, param2, param3);

                if (_currentRunningTask.Status == TaskStatus.Running
                   || _currentRunningTask.Status == TaskStatus.Created
                   || _currentRunningTask.Status == TaskStatus.WaitingToRun
                   || _currentRunningTask.Status == TaskStatus.WaitingForActivation
                   || _currentRunningTask.Status == TaskStatus.WaitingForChildrenToComplete)
                    await _currentRunningTask;
            }

            _recurringTaskCancelled = false;
        }

        public static void StopRunningProgram(bool clearPixels = true)
        {
            if (!ProgramRunning || _taskCancellationTokenSource.IsCancellationRequested && _ledCancellationTokenSource.IsCancellationRequested)
                return;
            try
            {
                _recurringTaskCancelled = true;
                _taskCancellationTokenSource.Cancel();
                _ledCancellationTokenSource.Cancel();

                while (_currentRunningTask.Status == TaskStatus.Running)
                {
                    Thread.Sleep(10);
                }

                if (clearPixels)
                {
                    _ledStrip.Clear();
                    _ledStrip.Render();
                }
                ProgramRunning = false;
            }
            catch (ObjectDisposedException) { }
        }
    }
}