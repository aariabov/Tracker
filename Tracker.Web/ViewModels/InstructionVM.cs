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

    public static IEnumerable<InstructionVm> CreateCollection(IEnumerable<Instruction> instructions)
    {
        var config = new MapperConfiguration(cfg => cfg.CreateMap<Instruction, InstructionVm>());
        var mapper = new Mapper(config);
        var vm = mapper.Map<IEnumerable<Instruction>, IEnumerable<InstructionVm>>(instructions);
        return vm;
    }
}
