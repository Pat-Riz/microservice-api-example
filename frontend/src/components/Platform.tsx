import { IPlatform } from "../App";

interface myProps {
  setId(id: number): void;
  setName(name: string): void;
  plat: IPlatform;
  selectedId: number;
}

const Platform = ({ setId, setName, plat, selectedId }: myProps) => {
  const bgColor =
    selectedId === plat.id ? "bg-gray-400" : "bg-gray-200 hover:bg-gray-300";

  return (
    <button
      onClick={() => {
        setId(plat.id);
        setName(plat.name);
      }}
      className={`animate-fadeIn w-auto p-3 h-auto m-2 border-2 border-opacity-70 rounded-xl border-slate-400 ${bgColor} hover:scale-105`}
      key={plat.id}
    >
      <div className='flex space-x-4 flex-col'>
        <h3 className='text-xl font-medium '>{plat.name}</h3>
        <div className='flex space-x-6'>
          <p className=''>{plat.publisher}</p>
          <p className=''>{plat.cost}</p>
        </div>
      </div>
    </button>
  );
};

export default Platform;
