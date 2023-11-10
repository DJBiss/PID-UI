namespace PidUI.Model
{
    public class Plant
    {
        private double y_prev = 0f;
        private double yd_prev = 0f;
        private double ydd_prev = 0f;
        private double u_prev = 0f;

        private readonly double dt = 0.02f;


        public double SimulatePlant(double u)
        {
            double ud = (u - u_prev) / dt;
            double yd = yd_prev + ydd_prev * dt;
            double y = y_prev + yd_prev * dt;

            double ydd = ud + 2 * u - yd - 2 * y;

            y_prev = y;
            yd_prev = yd;
            ydd_prev = ydd;
            u_prev = u;

            return y;
        }
    }
}
