export const allExamplePrompts = [
    "Show me all employees in the accounting department that are locked",
    "Create a bar chart of employees by city",
    "What is the average employee salary?",
    "List employees hired last year",
    "Who are the top 5 highest paid employees based on their monthly rate?",
    "Sum up Elden's mandatory government contributions(SSS, Philhealth, HDMF) from june last year up to date.",
    "What is the total number of employees?",
    "Find employees with 'Jake' in their name",
    "Display all available positions",
    "Show employee count per department",
    "List employees whose salary is above 25000",
    "Show the ratio of male and female locked employees, display a pie chart",
    "Who are the employees that have an ATM and are locked?",
    "Display a line chart of the top 5 employees with the highest net incomes from May to December last year, order it by payroll date(MM-dd-yyyy).",
    "What is the total salary expense for the entire company this month?",
    "Who is the highest-paid employee? Show their name and monthly rate.",
    "What is the total number of leave days taken last month?",
    "List each department and the total amount of overtime pay claimed last month.",
    "Show the monthly hiring trend for the last 12 months as a line chart.",
    "Compare the number of new hires versus resignations per month for the last year.",
    "Display our total salary expenses, quarter by quarter, for the last two years.",
    "List all employees who earn more than the company's average salary.",
    "Which department has the highest average employee tenure?",
    "Show me employees who have not taken any leave in the last 6 months.",
    "Count all employees by employment type 1 is Regular, 2 is Probationary, 3 is Newly Hired",
    "Generate a crosstab table showing the total gross income paid in each department, broken down by months in payroll for the first quarter this year. So the rows should have the department the columns are the months and value would be sum of gross income",
    "Generate a pivot table report showing the total gross income paid to employees in each department, broken down by months in payroll for the first quarter this year. So the rows should have the department as header the employees should be the details of the expanded department the columns are the months and value would be sum of gross income",
    "Generate a scatter chart to visualize the relationship between each employee's total number of months worked at the company (tenure) and their current annualized gross income. Each point on the chart should represent an individual employee. Use 'Total Number of Months Worked' for the X-axis. Use 'Current Annualized Gross Income' (in the relevant currency) for the Y-axis. This chart should help us understand if there's a general trend of gross income increasing with longer tenure and help identify any employees whose compensation seems notably high or low relative to their length of service, Select the top 10 employees",
    "Please draw a flowchart diagram illustrating the main steps involved in a typical monthly payroll processing cycle. Start from time data collection and end with payslip distribution and bank payments.",
    "Can you generate a tree view on admin and IT department (which is the root) and get the 10 of the records where I can navigate through employees(leafs) with their fullname, and monthly rate via position or payroll group(branches), Make sure to get at least one on every department",
    "I'd like to see a scatter chart for my top 10 oldest employees. Can you make sure their names are used to identify them on the chart (like in a legend)? For the plot itself, use their total years of service (from hire date) and their total accumulated salary from payroll as the two axes."
];

export let requestVerificationToken = '';

export function initializeConfig() {
    const tokenElement = document.getElementsByName('__RequestVerificationToken')[0];
    if (tokenElement) {
        requestVerificationToken = tokenElement.value;
    } else {
        console.error('__RequestVerificationToken not found in DOM.');
    }
}
