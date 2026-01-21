using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CommandManager<T> where T:MonoBehaviour
{
    protected T context;
    protected List<Command<T>> commandList;
    protected bool isBusy = false;

    public CommandManager(T context)
    {
        this.context = context;
        commandList = new List<Command<T>>();
        isBusy = false;
    }

    public void UpdateCommand()
    {
        if(!isBusy)
            return;
        for(int i=commandList.Count-1; i>=0; i--){
        //Note: The Command in the list might be swapped during Adding Command or Aborting Command.
        //Becareful not to Add Command while complete a command
            var command = commandList[i];
            if(command.IsPending) command.SetStatus(CommandStatus.Working);
            if(command.IsDone)
                HandleCompletion(command);
            else {
                command.CommandUpdate(context);
                if(command.IsDone) HandleCompletion(command);
            }
        }
    }
    public void AbortCommands(){
        for(int i=commandList.Count-1; i>=0; i--){
            var command = commandList[i];
            command.SetStatus(CommandStatus.Aborted);
            HandleCompletion(command);
        }
        commandList.Clear();
    }
    protected void HandleCompletion(Command<T> command){
        commandList.Remove(command);
        var nextCommand = command.GetNextCommand();
        if(nextCommand != null && command.IsSuccess){
            AddCommand(nextCommand);
        }
        command.SetStatus(CommandStatus.Detached);

        if(commandList.Count==0 || commandList == null) 
            isBusy = false;
    }
    public void AddCommand(Command<T> command){
        commandList.Add(command);
        command.SetStatus(CommandStatus.Pending);

        if(!isBusy) 
            isBusy = true;
    }
}