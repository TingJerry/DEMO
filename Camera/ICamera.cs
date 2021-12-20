namespace Camera       
{
    public interface ICamera
    {       
        int Connect();

        int Disconnect();

        int Live();

        int Grab();

        void Stop();
    }
}
