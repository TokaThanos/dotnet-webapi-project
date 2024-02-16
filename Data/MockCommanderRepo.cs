using System.Collections.Generic;
using Commander.Models;

namespace Commander.Data
{
    public class MockCommanderRepo : ICommanderRepo
    {
        public void CreateCommand(Command command)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteCommand(Command command)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Command> GetAllCommands()
        {
            var commands = new List<Command>
            {
                new Command { Id = 0, HowTo = "Drive a car", Line = "Pass the driving test", Platform = "Car" },
                new Command { Id = 1, HowTo = "Ride a horse", Line = "Make the horse calm", Platform = "Horse" },
                new Command { Id = 2, HowTo = "Play football", Line = "Discipline & hard work", Platform = "Ground" }
            };

            return commands;
        }

        public Command GetCommandById(int id)
        {
            return new Command { Id = 0, HowTo = "Drive a car", Line = "Pass the driving test", Platform = "Ground" };
        }

        public bool SaveChanges()
        {
            throw new System.NotImplementedException();
        }

        public void UpdateCommand(Command command)
        {
            throw new System.NotImplementedException();
        }
    }
}