export default function ErrorAlert({ message, onDismiss }) {
  if (!message) return null;

  return (
    <div className="alert alert-danger alert-dismissible d-flex align-items-center" role="alert">
      <span>{message}</span>
      {onDismiss && (
        <button type="button" className="btn-close ms-auto" aria-label="Dismiss" onClick={onDismiss} />
      )}
    </div>
  );
}
