namespace working_with_timescale_data.Models.ModelEntityConfig;

public class FileResultEntityConfig : IEntityTypeConfiguration<FileResultEntity>
{
    public void Configure(EntityTypeBuilder<FileResultEntity> builder)
    {
        builder.HasKey(x => x.Id);
    }
}
