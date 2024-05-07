namespace RaspPiBme.SonoffBasic.Controller
{
    public interface ISonoffBasicController
    {
        void SwitchSonoffBasic(double avg, string timeSeriesKey);
               
    }
}