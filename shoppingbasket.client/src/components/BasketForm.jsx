import "/src/styles/BasketForm.css";
import { useForm } from "react-hook-form";

function BasketForm() {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();

  //FIXME: change to calling HttpHelper for post receipt and render receipt from response
  const onSubmit = (data) => {
    console.log("data", data);
  };

  //TODO: move inputs to separate component with this signatures and errors handling
  return (
    <>
      {/*Shopping basket form*/}
      <form id="basket-form" onSubmit={handleSubmit(onSubmit)}>
        {/*Soup quantity input*/}
        <label htmlFor="soup-quantity">Soup (€0.65)</label>
        <input
          type="number"
          id="soup-quantity"
          defaultValue="0"
          className="quantity-inputs"
          {...register("soupQuantity", { required: true, min: 0 })}
          aria-invalid={errors.soupQuantity ? "true" : "false"}
        />
        {errors.soupQuantity?.type === "required" && (
          <p role="alert">Soup quantity is required!</p>
        )}
        {errors.soupQuantity?.type === "min" && (
          <p role="alert">Soup quantity should be greater than 0!</p>
        )}

        {/*Bread quantity input*/}
        <label htmlFor="bread-quantity">Bread (€0.8)</label>
        <input
          type="number"
          id="bread-quantity"
          defaultValue="0"
          className="quantity-inputs"
          {...register("breadQuantity", { required: true, min: 0 })}
          aria-invalid={errors.breadQuantity ? "true" : "false"}
        />
        {errors.breadQuantity?.type === "required" && (
          <p role="alert">Bread quantity is required!</p>
        )}
        {errors.breadQuantity?.type === "min" && (
          <p role="alert">Bread quantity should be greater than 0!</p>
        )}

        {/*Milk quantity input*/}
        <label htmlFor="milk-quantity">Milk (€1.3)</label>
        <input
          type="number"
          id="milk-quantity"
          defaultValue="0"
          className="quantity-inputs"
          {...register("milkQuantity", { required: true, min: 0 })}
          aria-invalid={errors.milkQuantity ? "true" : "false"}
        />
        {errors.milkQuantity?.type === "required" && (
          <p role="alert">Milk quantity is required!</p>
        )}
        {errors.milkQuantity?.type === "min" && (
          <p role="alert">Milk quantity should be greater than 0!</p>
        )}

        {/*Apples quantity input*/}
        <label htmlFor="apples-quantity">Apples (€1.00 per bag)</label>
        <input
          type="number"
          id="apples-quantity"
          defaultValue="0"
          className="quantity-inputs"
          {...register("applesQuantity", { required: true, min: 0 })}
          aria-invalid={errors.applesQuantity ? "true" : "false"}
        />
        {errors.applesQuantity?.type === "required" && (
          <p role="alert">Apples quantity is required!</p>
        )}
        {errors.applesQuantity?.type === "min" && (
          <p role="alert">Apples quantity should be greater than 0!</p>
        )}
        <input type="submit" id="button-submit-form" value="Submit" />
      </form>
    </>
  );
}

export default BasketForm;
