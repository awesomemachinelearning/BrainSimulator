using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GoodAI.Core.Nodes;
using GoodAI.Core.Task;

namespace GoodAI.BrainSimulator.Nodes
{
    public enum Enabled3State
    {
        AllEnabled,
        AllDisabled,
        Mixed
    }

    /// <summary>
    /// Represents a collection of tasks, all of the same type, gathered from a selection of nodes (again, all of the same type).
    /// See also the <see>NodeSelection</see> class.
    /// </summary>
    public class TaskSelection
    {
        internal TaskSelection(PropertyInfo taskPropInfo, List<MyWorkingNode> nodes)
        {
            if (nodes.Count == 0)
                throw new ArgumentException("Must not be empty", nameof(nodes));

            m_taskPropInfo = taskPropInfo;
            m_nodes = nodes;
        }

        public MyTask Task => TaskSpecimen;

        public IEnumerable<MyTask> EnumerateTasks() => m_nodes.Select(GetCurrentTask);

        public object[] ToObjectArray() => EnumerateTasks().Cast<object>().ToArray();

        public string Name => TaskSpecimen.Name;

        public bool OneShot => TaskSpecimen.OneShot;

        public Enabled3State Enabled3State
        {
            get
            {
                var enabledCount = EnumerateTasks().Count(t => t.Enabled);

                return (enabledCount == 0)
                    ? Enabled3State.AllDisabled
                    : ((enabledCount == m_nodes.Count)
                        ? Enabled3State.AllEnabled
                        : Enabled3State.Mixed);
            }
        }

        ///<summary>Returns true only if *all* tasks are enabled (see also Enabled3State property).</summary>
        public bool AllEnabled => (Enabled3State == Enabled3State.AllEnabled);

        /// <summary>Returns true if *any* of the tasks is Forbidden.</summary>
        public bool Forbidden => EnumerateTasks().Any(task => task.Forbidden);

        /// <summary>Returns true if *any* of the tasks is DesignTime.</summary>
        public bool DesignTime => EnumerateTasks().Any(task => task.DesignTime);

        public string TaskGroupName => TaskSpecimen.TaskGroupName;

        private MyTask TaskSpecimen => GetCurrentTask(m_nodes.First());

        private readonly PropertyInfo m_taskPropInfo;
        private readonly List<MyWorkingNode> m_nodes;

        private MyTask GetCurrentTask(MyWorkingNode node) => node.GetTaskByPropertyName(m_taskPropInfo.Name);
    }
}