using System;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP.Runtime.Core
{
   
   public abstract class ActionPlanner
   {
      private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>();
      public Dictionary<string, object> Parameters => _parameters;

      public abstract int Calculate(out List<BaseAction> actions);
   }
      
   public abstract class BaseAction : ScriptableObject
   {
      private ActionPlanner _actionPlanner;
      
      public enum Result {Success, Running, Failure }

      private bool _started = false;

      public List<Condition> Preconditions { get; } = null;

      public List<Consequence> Consequences { get; } = null;

      protected Dictionary<string, object> Parameters => _actionPlanner.Parameters;

      /// <summary>
      /// Calculates de estimated cost of the action if the action had to be run useing the given parameters
      /// </summary>
      /// <param name="parameters"></param>
      /// <returns></returns>
      public int CalculateCost(Dictionary<string, object> parameters)
      {
         return 0;
      }

      public void Initialize(ActionPlanner actionPlanner)
      {
         _actionPlanner = actionPlanner;
         Initialize();
      }

      protected abstract void Initialize();
      /// <summary>
      /// Executes when the state becomes active
      /// </summary>
      protected abstract void Enter();

      /// <summary>
      /// Executes when the state returns SuccessOrFailure
      /// </summary>
      protected abstract void Exit();

      public Result Execute()
      {
         if (!_started)
         {
            _started = true;
            Enter();
         }
      
         Result result =  Execution();
         if (result != Result.Running)
         {
            Exit();
            _started = false;
         }
         return result;
      }

      protected abstract Result Execution();

      
      /// <summary>
      /// Checks all the preconditions
      /// </summary>
      /// <returns>True: if all preconditions are true</returns>
      public bool IsReady() => Preconditions.TrueForAll(x => x.Validate(Parameters));

   }





   public abstract class Condition : ScriptableObject
   {
      public abstract bool Validate(Dictionary<string, object> parameters);
   }

   public abstract class Consequence : ScriptableObject
   {
      public abstract void Execute(Dictionary<string, object> parameters);
   }

   public abstract class FitnessCalculator<T>
   {
      
   }

   public struct ActionParameter
   {
      public string id;
      public string value;
      public Type type;
   }
}
