using AppointmentsApi.Api.Features.Appointments;

namespace AppointmentsApi.IntegrationTests.Common;

public class ReserveAppointmentSlotApiRequestBuilder
{
    private DateTime _start;
    private DateTime _end;
    private string _comments = string.Empty;
    private ReserveppointmentSlottPatientApiRequest _patient = new();
    private Guid _facilityId;

    public ReserveAppointmentSlotApiRequestBuilder WithStart(DateTime start)
    {
        _start = start;
        return this;
    }

    public ReserveAppointmentSlotApiRequestBuilder WithEnd(DateTime end)
    {
        _end = end;
        return this;
    }

    public ReserveAppointmentSlotApiRequestBuilder WithComments(string comments)
    {
        _comments = comments;
        return this;
    }

    public ReserveAppointmentSlotApiRequestBuilder WithPatient(ReserveppointmentSlottPatientApiRequest patient)
    {
        _patient = patient;
        return this;
    }

    public ReserveAppointmentSlotApiRequestBuilder WithPatient(Action<ReserveAppointmentSlottPatientApiRequestBuilder> patientBuilder)
    {
        var builder = new ReserveAppointmentSlottPatientApiRequestBuilder();
        patientBuilder.Invoke(builder);
        return WithPatient(builder.Build());
    }

    public ReserveAppointmentSlotApiRequestBuilder WithFacilityId(Guid facilityId)
    {
        _facilityId = facilityId;
        return this;
    }

    public ReserveAppointmentSlotApiRequest Build()
    {
        return new ReserveAppointmentSlotApiRequest
        {
            Start = _start,
            End = _end,
            Comments = _comments,
            Patient = _patient,
            FacilityId = _facilityId
        };
    }
}

public class ReserveAppointmentSlottPatientApiRequestBuilder
{
    private string _name = string.Empty;
    private string _secondName = string.Empty;
    private string _email = string.Empty;
    private string _phone = string.Empty;

    public ReserveAppointmentSlottPatientApiRequestBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ReserveAppointmentSlottPatientApiRequestBuilder WithSecondName(string secondName)
    {
        _secondName = secondName;
        return this;
    }

    public ReserveAppointmentSlottPatientApiRequestBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public ReserveAppointmentSlottPatientApiRequestBuilder WithPhone(string phone)
    {
        _phone = phone;
        return this;
    }

    public ReserveppointmentSlottPatientApiRequest Build()
    {
        return new ReserveppointmentSlottPatientApiRequest
        {
            Name = _name,
            SecondName = _secondName,
            Email = _email,
            Phone = _phone
        };
    }
}