const deleteBtns = document.getElementsByClassName("delete-button");

for (let deleteBtn of deleteBtns)
{
    deleteBtn.onclick = async function()
    {
        if (confirm("Are you sure?"))
        {
            await fetch(`${window.location.origin}/api/deleteSocialMedia`, {
                method: 'DELETE',
                body: JSON.stringify({
                    socialNetworkName: deleteBtn.id
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

const socialMediaInput = document.getElementById("social-media-input");
const linkInput = document.getElementById("link-input");
const addBtn = document.getElementById("add-button");

addBtn.onclick = async function()
{
    const socialNetworkName = socialMediaInput.value;
    const link = linkInput.value;

    await fetch(`${window.location.origin}/api/addSocialMedia`, {
        method: 'POST',
        body: JSON.stringify({
            socialNetworkName,
            link
        }),
        headers: {
            'content-type': 'application/json'
        }
    })
    .then((response) => {
        if (response.ok)
        {
            CreateRow(socialNetworkName, link);
        }
        else
        {
            alert("commnad already exists!")
        }
    })

    socialMediaInput.value = "";
    linkInput.value = "";
}

function CreateRow(socialNetworkName, link)
{
    let newRow = document.createElement("tr");
    newRow.Id = `row-${socialNetworkName}`;

    let socialNetworkNameColumn = document.createElement("th");
    socialNetworkNameColumn.innerHTML = socialNetworkName;
    let linkColumn = document.createElement("th");
    linkColumn.innerHTML = link;
    let deleteColumn = document.createElement("th");
    let deleteBtn = document.createElement("div");
    deleteBtn.className = "delete-button";
    deleteBtn.innerHTML = "delete";

    deleteBtn.onclick = async function()
    {
        if (confirm("Are you sure?"))
        {
            await fetch(`${window.location.origin}/api/deleteSocialMedia`, {
                method: 'DELETE',
                body: JSON.stringify({
                    socialNetworkName
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

    newRow.appendChild(socialNetworkNameColumn);
    newRow.appendChild(linkColumn);
    newRow.appendChild(deleteColumn);

    document.getElementById("table-body").insertBefore(newRow, document.getElementById("add-row"));
}