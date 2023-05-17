const deleteBtns = document.getElementsByClassName("delete-button");

for (let deleteBtn of deleteBtns)
{
    deleteBtn.onclick = async function()
    {
        if (confirm("Are you sure?"))
        {
            await fetch(`${window.location.origin}/api/deleteCommand`, {
                method: 'DELETE',
                body: JSON.stringify({
                    commandName: deleteBtn.id
                }),
                headers: {
                    'content-type': 'application/json'
                }
            })
            .then((response) => {
                if (response.ok)
                {
                    document.getElementById(`row-${deleteBtn.id}`).remove();
                }
                else
                {
                    alert("something went wrong")
                }
            });            
        }
    }
}

const commadInput = document.getElementById("command-input");
const outputInput = document.getElementById("output-input");
const addBtn = document.getElementById("add-button");

addBtn.onclick = async function()
{
    const commandName = commadInput.value;
    const commandOutput = outputInput.value;

    await fetch(`${window.location.origin}/api/addCommand`, {
        method: 'POST',
        body: JSON.stringify({
            commandName,
            commandOutput
        }),
        headers: {
            'content-type': 'application/json'
        }
    })
    .then((response) => {
        if (response.ok)
        {
            CreateRow(commandName, commandOutput);
        }
        else
        {
            alert("commnad already exists!")
        }
    })

    commadInput.value = "";
    outputInput.value = "";
}

function CreateRow(commandName, commandOutput)
{
    let newRow = document.createElement("tr");
    newRow.Id = `row-${commandName}`;

    let commandNameColumn = document.createElement("th");
    commandNameColumn.innerHTML = commandName;
    let commandOutputColumn = document.createElement("th");
    commandOutputColumn.innerHTML = commandOutput;
    let deleteColumn = document.createElement("th");
    let deleteBtn = document.createElement("div");
    deleteBtn.className = "delete-button";
    deleteBtn.innerHTML = "delete";

    deleteBtn.onclick = async function()
    {
        if (confirm("Are you sure?"))
        {
            await fetch(`${window.location.origin}/api/deleteCommand`, {
                method: 'DELETE',
                body: JSON.stringify({
                    commandName
                }),
                headers: {
                    'content-type': 'application/json'
                }
            })
            .then((response) => {
                if (response.ok)
                {
                    newRow.remove();
                }
                else
                {
                    alert("something went wrong")
                }
            });            
        }
    }

    deleteColumn.appendChild(deleteBtn);

    newRow.appendChild(commandNameColumn);
    newRow.appendChild(commandOutputColumn);
    newRow.appendChild(deleteColumn);

    document.getElementById("table-body").insertBefore(newRow, document.getElementById("add-row"));
}