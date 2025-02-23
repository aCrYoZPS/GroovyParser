class Task {
    String title
    boolean completed = false

    Task(String title) {
        this.title = title
    }

    void complete() {
        this.completed = true
    }

    String toString() {
        return "[${completed ? 'X' : ' '}] ${title}"
    }
}

class ToDoList {
    List<Task> tasks = []

    void addTask(String title) {
        tasks << new Task(title)
        println "Task added: '${title}'"
    }

    void removeTask(int index) {
        if (index in 0..<tasks.size()) {
            println "Task removed: '${tasks[index].title}'"
            tasks.remove(index)
        } else {
            println "Invalid task number."
        }
    }

    void completeTask(int index) {
        if (index in 0..<tasks.size()) {
            tasks[index].complete()
            println "Task completed: '${tasks[index].title}'"
        } else {
            println "Invalid task number."
        }
    }

    void showTasks() {
        if (tasks.isEmpty()) {
            println "No tasks available."
        } else {
            tasks.eachWithIndex { task, index -> println "${index + 1}. ${task}" }
        }
    }
}

class ToDoApp {
    static void main(String[] args) {
        ToDoList toDoList = new ToDoList()
        Scanner scanner = new Scanner(System.in)
        
        while (true) {
            println "\nTo-Do List Menu:\n1. Add Task\n2. Remove Task\n3. Complete Task\n4. Show Tasks\n5. Exit"
            print "Choose an option: "
            String choice = scanner.nextLine()

            switch (choice) {
                case '1':
                    print "Enter task title: "
                    String title = scanner.nextLine()
                    toDoList.addTask(title)
                    break
                case '2':
                    print "Enter task number to remove: "
                    try {
                        int index = Integer.parseInt(scanner.nextLine()) - 1
                        toDoList.removeTask(index)
                    } catch (NumberFormatException e) {
                        println "Invalid input."
                    }
                    break
                case '3':
                    print "Enter task number to complete: "
                    try {
                        int index = Integer.parseInt(scanner.nextLine()) - 1
                        toDoList.completeTask(index)
                    } catch (NumberFormatException e) {
                        println "Invalid input."
                    }
                    break
                case '4':
                    toDoList.showTasks()
                    break
                case '5':
                    println "Exiting..."
                    return
                default:
                    println "Invalid option, try again."
            }
        }
    }
}
