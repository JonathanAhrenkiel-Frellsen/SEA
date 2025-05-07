using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.CodeGeneration.TypeScript;
using Survey.Application;

var settings = new JsonSchemaGeneratorSettings
{
    DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.Null,
    FlattenInheritanceHierarchy = true
};

var generator = new JsonSchemaGenerator(settings);

var dtoTypes = new[]
{
    typeof(DesignedSurveyDto),
    typeof(LoginDto),
    typeof(RegisterUserDto),
    typeof(UserDto),
    typeof(JwtSettings),
    typeof(ExportSurvey),
    typeof(DesignedSurveyDto),
    typeof(ImportSurveyDto),
    typeof(SurveySaveAnswerDto),
    typeof(ExperimenteeAppDto)
};

Directory.CreateDirectory("../../frontend/src/shared/dto");

foreach (var type in dtoTypes)
{
    var schema = generator.Generate(type);
    var tsGen = new TypeScriptGenerator(schema, new TypeScriptGeneratorSettings
    {
        TypeStyle = TypeScriptTypeStyle.Interface,
        NullValue = TypeScriptNullValue.Undefined,
        GenerateConstructorInterface = false
    });

    var tsCode = tsGen.GenerateFile();
    var fileName = $"{type.Name}.ts";

    File.WriteAllText(Path.Combine("../../frontend/src/shared/dto", fileName), tsCode);
    Console.WriteLine($"✅ Wrote {fileName}");
}
