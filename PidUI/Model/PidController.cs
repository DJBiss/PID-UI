namespace PidUI.Model
{
    class PidController
    {
        public double KP { get; set; }
        public double KI { get; set; }
        public double KD { get; set; }

        private readonly double dt;
        private readonly bool useMeasurement;
        private readonly bool rateSupplied;

        private double previousError;
        private double previousInput;
        private double integralAccumulation;

        /// <summary>
        /// PID constructor
        /// </summary>
        /// <param name="timeStep"> change in time in seconds </param>
        /// <param name="useMeasurement"> if True, use the measurement to calculate the derivative output, as opposed to the error </param>
        /// <param name="rateSupplied"> if True, use the rate measurement that is supplied, else calculate it discretely </param>
        public PidController(double timeStep, bool useMeasurement, bool rateSupplied)
        {
            dt = timeStep;
            this.useMeasurement = useMeasurement;
            this.rateSupplied = rateSupplied;
        }

        /// <summary>
        /// Calculate output of the PID at current timestep 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="current"></param>
        /// <param name="suppliedRate"></param>
        /// <returns></returns>
        public double CalculateOutput(double target, double current, double suppliedRate = 0d)
        {
            double error = target - current;
            double cp = KP * error;

            integralAccumulation += error * dt;
            double ci = KI * integralAccumulation;

            double cd;

            if (!useMeasurement)
            {
                // derivative on error
                double errorRate = (error - previousError) / dt;
                cd = KD * errorRate;
            }

            else
            {
                // derivative on measurement
                if (rateSupplied)
                {
                    // use the measurement rate directly
                    cd = KD * suppliedRate;
                }

                else
                {
                    // calculate the discrete rate of change of the measurement and use that
                    double measurementRate = (current - previousInput) / dt;
                    cd = KD * measurementRate;
                }
            }

            previousError = error;
            previousInput = current;

            return cp + ci - cd;
        }


    }
}
