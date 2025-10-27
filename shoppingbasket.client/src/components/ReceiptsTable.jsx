import { useEffect, useState } from "react";
import "/src/styles/ReceiptsTable.css";
import { httpService } from "../utils/httpService";

function ReceiptsTable() {
  const [receipts, setReceipts] = useState();

  useEffect(() => {
    populateReceiptsData();
  }, []);

  //TODO: move this to separate receiptsService based on httpService with defined requests
  async function populateReceiptsData() {
    try {
      const data = await httpService.get("/receipts");
      setReceipts(data);
    } catch (error) {
      console.log("Error fetching receipts", error);
    }
  }

  const contents =
    receipts === undefined ? (
      <p>
        <em>Loading... Please wait</em>
      </p>
    ) : (
      <table className="table table-striped" aria-labelledby="tableLabel">
        <thead>
          <tr>
            <th>Receipt Number</th>
            <th>Total Cost (€) </th>
            <th>Created Date</th>
          </tr>
        </thead>
        <tbody>
          {receipts.map((receipt) => (
            <tr key={receipt.receiptId}>
              <td>{receipt.receiptNumber}</td>
              <td>{receipt.totalCost}</td>
              {/*FIXME: display only date like dd/mm/yyyy
                      convert full date from API to this format              
              */}
              <td>{Date(receipt.createdDateTime)}</td>
            </tr>
          ))}
        </tbody>
      </table>
    );

  return (
    <div>
      <h1 id="tableLabel">History of receipts</h1>
      <p>This component contains completed transactions</p>
      {contents}
    </div>
  );
}

export default ReceiptsTable;
