using NumSharp;
using System.Collections.Generic;

namespace PidUI.Model
{
    /// <summary>
    /// Create and run a model with a PID controller and a Plant model
    /// </summary>
    class PidSystem
    {
        private readonly Plant systemPlant;
        private readonly PidController pid;

        public PidSystem(Plant systemPlant, PidController pid)
        {
            this.systemPlant = systemPlant;
            this.pid = pid;
        }

        /// <summary>
        /// Run the PID System simulation
        /// </summary>
        /// <param name="timeVec"> NDArray with timesteps to simulate system at </param>
        /// <returns></returns>
        public double[] RunSimulation(NDArray timeVec)
        {
            List<double> timeStamps = new(); // to store the timesteps at each iteration
            List<double> sysOutputs = new(); // to store the outputs at each iteration

            double y_i = 0f; // initial condition
            for (int i = 0; i < timeVec.size; i++)
            {
                double pidOutput = pid.CalculateOutput(target: 1f, current: y_i);
                y_i = systemPlant.SimulatePlant(pidOutput);

                timeStamps.Add(timeVec[i]);
                sysOutputs.Add(y_i);
            }

            double[] sysOutputArray = sysOutputs.ToArray();
            return sysOutputArray;
        }
    }
}
