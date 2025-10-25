import "/src/styles/BasketForm.css";

export default function QuantityInput(props) {
  return (
    <>
      <p>{props.title}</p>
      <input
        type="number"
        className={props.inputClassName}
        data-label="Item"
        min="0"
      />
    </>
  );
}
