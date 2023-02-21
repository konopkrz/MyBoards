﻿using System.ComponentModel.DataAnnotations;

namespace MyBoards.Entities
{
    public class Epic : WorkItem
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class Issue : WorkItem
    {
        public decimal Effort { get; set; }
    }

    public class Task : WorkItem
    {

        public string Activity { get; set; }
        public decimal RemainingWork { get; set; }
    }
    public abstract class WorkItem
    {

        public int Id { get; set; }

        public WorkItemState State { get; set; }

        public int StateId { get; set; }

        public string Area { get; set; }
        public string IterationPath { get; set; }

        public int Prority { get; set; }

        public List<Comment> Comments { get; set; } = new List<Comment>();

        public User Author { get; set; }

        public Guid AuthorId { get; set; }

        public List<Tag> Tags { get; set; }


    }
}
