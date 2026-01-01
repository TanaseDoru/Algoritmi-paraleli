using System.Collections.Concurrent;

namespace ex02
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                List<PersonNode> roots = InitPeopleGraph();

                var allNodes = new HashSet<PersonNode>();
                CollectAllNodes(roots, allNodes);

                var remainingParents = new Dictionary<PersonNode, int>();
                foreach (var node in allNodes)
                {
                    int parentCount = 0;
                    foreach (var other in allNodes)
                    {
                        if (other.Friends.Contains(node))
                            parentCount++;
                    }
                    remainingParents[node] = parentCount;
                }

                var readyQueue = new ConcurrentQueue<PersonNode>(
                    remainingParents.Where(kvp => kvp.Value == 0).Select(kvp => kvp.Key));

                var processingTasks = new List<Task>();

                while (readyQueue.TryDequeue(out var node) || processingTasks.Any())
                {
                    while (readyQueue.TryDequeue(out var readyNode))
                    {
                        var task = Task.Run(async () =>
                        {
                            await Process(readyNode);

                            foreach (var friend in readyNode.Friends)
                            {
                                int current = remainingParents[friend];
                                int newRemaining = Interlocked.Decrement(ref current);
                                remainingParents[friend] = newRemaining;

                                if (newRemaining == 0)
                                {
                                    readyQueue.Enqueue(friend);
                                }
                            }
                            

                        });

                        processingTasks.Add(task);
                    }

                    if (processingTasks.Any())
                    {
                        await Task.WhenAny(processingTasks);
                        processingTasks = processingTasks.Where(t => !t.IsCompleted).ToList();
                    }
                }

                await Task.WhenAll(processingTasks); 
                Console.WriteLine("Traversarea paralelă pe niveluri a fost finalizată.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void CollectAllNodes(List<PersonNode> roots, HashSet<PersonNode> allNodes)
        {
            var queue = new Queue<PersonNode>(roots);
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                if (allNodes.Add(node))
                {
                    foreach (var friend in node.Friends)
                    {
                        queue.Enqueue(friend);
                    }
                }
            }
        }

        #region DO_NOT_MODIFY_THIS

        static async Task Process(PersonNode node)
        {
            Random random = new Random();
            int no_it = random.Next(1, 10);

            for (int it = 1; it <= no_it; it++)
            {
                await Task.Delay(random.Next(1, 20) * 100);
                Console.WriteLine($"Processing [{node.Name}] {it}/{no_it}");
            }
        }


        static List<PersonNode> InitPeopleGraph()
        {
            List<PersonNode> people = new List<PersonNode>()
            {
                new PersonNode()
                {
                    Name = "Emma Johnson",
                    Friends = new List<PersonNode>()
                    {
                        new PersonNode()
                        {
                            Name = "William Thompson",
                            Friends = new List<PersonNode>()
                            {
                                new PersonNode()
                                {
                                    Name = "Mia Taylor",
                                    Friends = new List<PersonNode>()
                                    {
                                        new PersonNode()
                                        {
                                            Name = "Liam Adams"
                                        },
                                        new PersonNode()
                                        {
                                            Name = "Charlotte Lewis"
                                        },
                                        new PersonNode()
                                        {
                                            Name = "Noah Brooks"
                                        }
                                    }
                                },
                                new PersonNode()
                                {
                                    Name = "Ethan Mitchell",
                                    Friends = new List<PersonNode>()
                                    {
                                        new PersonNode()
                                        {
                                            Name = "Emily Powell",
                                            Friends = new List<PersonNode>()
                                            {
                                                new PersonNode()
                                                {
                                                    Name = "Lucas Reed"
                                                },
                                                new PersonNode()
                                                {
                                                    Name = "Lily Simmons",
                                                    Friends = new List<PersonNode>()
                                                    {
                                                        new PersonNode()
                                                        {
                                                            Name = "Daniel Hayes"
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                },
                                new PersonNode()
                                {
                                    Name = "Isabella Wright"
                                },
                                new PersonNode()
                                {
                                    Name = "Amelia Davis",
                                    Friends = new List<PersonNode>()
                                    {
                                        new PersonNode()
                                        {
                                            Name = "Henry Wood"
                                        },
                                        new PersonNode()
                                        {
                                            Name = "Harper Morris",
                                            Friends= new List<PersonNode>()
                                            {
                                                new PersonNode()
                                                {
                                                    Name = "Jackson Riley"
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        },
                        new PersonNode()
                        {
                            Name = "Ava Turner",
                            Friends = new List<PersonNode>()
                            {
                                new PersonNode()
                                {
                                    Name = "Grace Palmer"
                                },
                                new PersonNode()
                                {
                                    Name = "Aiden Barnes"
                                }
                            }
                        },
                        new PersonNode()
                        {
                            Name = "Benjamin Foster"
                        }
                    }
                },
                new PersonNode()
                {
                    Name = "Alexander Martinez",
                    Friends = new List<PersonNode>()
                    {
                        new PersonNode()
                        {
                            Name = "Sophia Rodriguez"
                        },
                        new PersonNode()
                        {
                            Name = "Samuel Hernandez"
                        },
                        new PersonNode()
                        {
                            Name = "Chloe Evans",
                            Friends = new List<PersonNode>()
                            {
                                new PersonNode()
                                {
                                    Name = "Zoey Foster",
                                    Friends = new List<PersonNode>()
                                    {
                                        new PersonNode()
                                        {
                                            Name = "Logan Harrison"
                                        },
                                        new PersonNode()
                                        {
                                            Name = "Penelope Clark"
                                        }
                                    }
                                }
                            }
                        },
                        new PersonNode()
                        {
                            Name = "Oliver Wright"
                        }
                    }
                },
                new PersonNode()
                {
                    Name = "Olivia Baker",
                    Friends = new List<PersonNode>()
                    {
                        new PersonNode()
                        {
                            Name = "James Carter",
                            Friends = new List<PersonNode>()
                            {
                                new PersonNode()
                                {
                                    Name = "Mason Bennett"
                                },
                                new PersonNode()
                                {
                                    Name = "Abigail Ramirez"
                                },
                                new PersonNode()
                                {
                                    Name = "Elijah Watson",
                                    Friends = new List<PersonNode>()
                                    {
                                        new PersonNode()
                                        {
                                            Name = "Ella Patterson"
                                        },
                                        new PersonNode()
                                        {
                                            Name = "Caleb Jenkins",
                                            Friends = new List<PersonNode>()
                                            {
                                                new PersonNode()
                                                {
                                                    Name = "Samuel Dixon"
                                                },
                                                new PersonNode()
                                                {
                                                    Name = "Sophia Griffin"
                                                },
                                                new PersonNode()
                                                {
                                                    Name = "Carter Reynolds"
                                                }
                                            }
                                        },
                                        new PersonNode()
                                        {
                                            Name = "Ava Parker"
                                        }
                                    }
                                }
                            }
                        }
                    }
                },
            };

            return people;
        }

        #endregion
    }
}