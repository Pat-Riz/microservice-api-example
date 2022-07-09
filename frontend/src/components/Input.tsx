interface myProps {
  name: string;
  value: string;
  onChange: (value: string) => void;
  required?: boolean;
}

export default function Input({ onChange, name, value, required }: myProps) {
  const requiredCss = required ? "after:content-['*']" : "";
  return (
    <div className={`flex ${requiredCss}`}>
      {/* <p className='mr-2'>{name}</p> */}
      <input
        value={value}
        onChange={(e) => onChange(e.target.value)}
        className='border p-1 m-2 border-slate-500 w-36'
        placeholder={name}
        required={required}
      />
    </div>
  );
}
