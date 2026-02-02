namespace working_with_timescale_data.Models.ModelEntityConfig;


public class MeasurementEntityConfig : IEntityTypeConfiguration<MeasurementEntity>
{
    public void Configure(EntityTypeBuilder<MeasurementEntity> builder)
    {
        builder.HasKey(m => m.Id);
    }
}
