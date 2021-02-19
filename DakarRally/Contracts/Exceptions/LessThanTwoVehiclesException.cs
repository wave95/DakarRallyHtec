using System;

namespace DakarRally.Contracts.Exceptions
{
    public class LessThanTwoVehiclesException : Exception
    {
        public LessThanTwoVehiclesException() : base(Domain.Constants.Races.AtLeastTwoVehicles)
        {
        }
    }
}
