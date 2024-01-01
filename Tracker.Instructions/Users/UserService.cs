using Tracker.Instructions.Kafka;

namespace Tracker.Instructions;

public class UserService
{
    private readonly UserRepository _userRepository;

    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task UpdateUser(KafkaUser kafkaUser)
    {
        var user = kafkaUser.ToUser();
        _userRepository.UpdateUser(user);
        await _userRepository.SaveChangesAsync();
    }

    public async Task InsertUser(KafkaUser kafkaUser)
    {
        var user = kafkaUser.ToUser();
        _userRepository.InsertUser(user);
        await _userRepository.SaveChangesAsync();
    }
}
