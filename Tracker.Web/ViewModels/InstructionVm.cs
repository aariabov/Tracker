using AutoMapper;
using Tracker.Web.Domain;

namespace Tracker.Web.ViewModels;

public class InstructionVm
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentId { get; set; }
    public string CreatorName { get; set; }
    public string ExecutorName { get; set; }
    public DateTime Deadline { get; set; }
    public DateTime? ExecDate { get; set; }
    public string Status { get; set; }
    public bool CanCreateChild { get; set; }
    public bool CanBeExecuted { get; set; }

    public static IEnumerable<InstructionVm> CreateCollection(IEnumerable<Instruction> instructions)
    {
        return instructions.Select(i => Create(i, canCreateChild: false, canBeExecuted: false));

        // var config = new MapperConfiguration(cfg =>
        // {
        //     cfg.CreateMap<Instruction, InstructionVm>()
        //         .ForMember(dest => dest.ExecutorName, opt => opt.MapFrom(src => src.Executor.UserName));
        // });
        // var mapper = new Mapper(config);
        // var vm = mapper.Map<IEnumerable<Instruction>, IEnumerable<InstructionVm>>(instructions);
        // return vm;
    }
    
    public static InstructionVm Create(Instruction instruction, bool canCreateChild, bool canBeExecuted)
    {
        return new InstructionVm
        {
            Id = instruction.Id
            , Name = instruction.Name
            , ParentId = instruction.ParentId
            , CreatorName = instruction.Creator.UserName
            , ExecutorName = instruction.Executor.UserName
            , Deadline = instruction.Deadline
            , ExecDate = instruction.ExecDate
            , Status = GetStatus(instruction.Status)
            , CanCreateChild = canCreateChild
            , CanBeExecuted = canBeExecuted
        };
    }

    private static string GetStatus(ExecStatus status)
    {
        switch (status)
        {
            case ExecStatus.InWork: return "В работе";
            case ExecStatus.InWorkOverdue: return "В работе просрочено";
            case ExecStatus.Completed: return "Выполнено в срок";
            case ExecStatus.CompletedOverdue: return "Выполнено с нарушением срока";
            default: throw new Exception("Unknown ExecStatus");
        }
    }
}
