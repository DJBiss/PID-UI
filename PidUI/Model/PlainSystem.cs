using NumSharp;
using System.Collections.Generic;

namespace PidUI.Model
{
    class PlainSystem
    {
        private readonly Plant plant;
        public PlainSystem(Plant plant)
        {
            this.plant = plant;
        }

        public double[] RunSimulation(NDArray timeVec)
        {
            List<double> timeStamps = new List<double>(); // to store the timesteps at each iteration
            List<double> sysOutputs = new List<double>(); // to store the outputs at each iteration

            for (int i = 0; i < timeVec.size; i++)
            {
                double y_i = plant.SimulatePlant(1f);

                timeStamps.Add(timeVec[i]);
                sysOutputs.Add(y_i);
            }

            double[] sysOutputArray = sysOutputs.ToArray();
            return sysOutputArray; // return the outputs array to do a multi plot
        }
    }
}
