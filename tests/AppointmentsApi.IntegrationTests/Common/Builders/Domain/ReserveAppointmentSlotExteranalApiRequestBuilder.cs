using AppointmentsApi.Domain.Services;

namespace AppointmentsApi.IntegrationTests.Common;

public class ReserveAppointmentSlotExteranalApiRequestBuilder
{
    private DateTime _start;
    private DateTime _end;
    private string _comments = string.Empty;
    private ReserveAppointmentSlotPatientExteranalApiRequest _patient = new();
    private Guid _facilityId;

    public ReserveAppointmentSlotExteranalApiRequestBuilder WithStart(DateTime start)
    {
        _start = start;
        return this;
    }

    public ReserveAppointmentSlotExteranalApiRequestBuilder WithEnd(DateTime end)
    {
        _end = end;
        return this;
    }

    public ReserveAppointmentSlotExteranalApiRequestBuilder WithComments(string comments)
    {
        _comments = comments;
        return this;
    }

    public ReserveAppointmentSlotExteranalApiRequestBuilder WithPatient(ReserveAppointmentSlotPatientExteranalApiRequest patient)
    {
        _patient = patient;
        return this;
    }

    public ReserveAppointmentSlotExteranalApiRequestBuilder WithPatient(Action<ReserveAppointmentSlotPatientExteranalApiRequestBuilder> patientBuilder)
    {
        var builder = new ReserveAppointmentSlotPatientExteranalApiRequestBuilder();
        patientBuilder.Invoke(builder);
        return WithPatient(builder.Build());
    }

    public ReserveAppointmentSlotExteranalApiRequestBuilder WithFacilityId(Guid facilityId)
    {
        _facilityId = facilityId;
        return this;
    }

    public ReserveAppointmentSlotExteranalApiRequest Build()
    {
        return new ReserveAppointmentSlotExteranalApiRequest
        {
            Start = _start,
            End = _end,
            Comments = _comments,
            Patient = _patient,
            FacilityId = _facilityId
        };
    }
}

public class ReserveAppointmentSlotPatientExteranalApiRequestBuilder
{
    private string _name = string.Empty;
    private string _secondName = string.Empty;
    private string _email = string.Empty;
    private string _phone = string.Empty;

    public ReserveAppointmentSlotPatientExteranalApiRequestBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ReserveAppointmentSlotPatientExteranalApiRequestBuilder WithSecondName(string secondName)
    {
        _secondName = secondName;
        return this;
    }

    public ReserveAppointmentSlotPatientExteranalApiRequestBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public ReserveAppointmentSlotPatientExteranalApiRequestBuilder WithPhone(string phone)
    {
        _phone = phone;
        return this;
    }

    public ReserveAppointmentSlotPatientExteranalApiRequest Build()
    {
        return new ReserveAppointmentSlotPatientExteranalApiRequest
        {
            Name = _name,
            SecondName = _secondName,
            Email = _email,
            Phone = _phone
        };
    }
}