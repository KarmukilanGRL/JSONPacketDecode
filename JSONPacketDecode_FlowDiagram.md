# JSON Packet Decode Flow Diagram

This document contains Mermaid flow diagrams that visualize the JSON packet decoding process implemented in the JSONPacketDecode project.

## Main Process Flow

```mermaid
flowchart TD
    A[Start Program] --> B[Initialize DataDecoder]
    B --> C[Create FwByteData List]
    C --> D[Add Test Bytes: 0x25, 0x31, 0x42]
    D --> E[Read SampleJSON.json File]
    E --> F[Deserialize JSON to RootObject]
    F --> G[Loop Through Each Packet in JSON]
    
    G --> H{Does Header Match<br/>FwByteData[0]?}
    H -->|No| G
    H -->|Yes| I[Create DisplayPayloadvalues List]
    
    I --> J[Call DecoderMethod<br/>to get BitRange Dictionary]
    J --> K{FwByteData.Count > 1?}
    K -->|No| G
    K -->|Yes| L[Print Packet Name]
    
    L --> M[Loop Through Each Field<br/>in BitRange Dictionary]
    M --> N[Extract Payload Name<br/>and BitInfo Dictionary]
    N --> O[Find Corresponding Field<br/>in Packet.Fields]
    
    O --> P{BitInfoDict.Count == 1?}
    P -->|Yes| Q[Call PayloadDecodeShifting<br/>for Single Byte]
    P -->|No| R[Call PayloadDecodeShiftingForMultipleBytes<br/>for Multiple Bytes]
    
    Q --> S[Get strbitorder and itemp]
    R --> T[Get strbitorder and itempdata<br/>Convert to byte]
    
    S --> U[Check if Field has Values<br/>Call GetValueInfo]
    T --> U
    
    U --> V{Values Found?}
    V -->|Yes| W[Extract Description<br/>and Display from Values]
    V -->|No| X[Set Empty Description<br/>and Display]
    
    W --> Y[Add to DisplayPayloadvalues]
    X --> Y
    
    Y --> Z[Print Field Information:<br/>- Payload Name<br/>- Bit Order<br/>- Decoded Value<br/>- Description (if available)<br/>- Display (if available)]
    
    Z --> AA{More Fields<br/>to Process?}
    AA -->|Yes| M
    AA -->|No| BB[Print Display String<br/>with All Payload Values]
    
    BB --> CC{More Packets<br/>to Process?}
    CC -->|Yes| G
    CC -->|No| DD[End Program]

    style A fill:#90EE90
    style DD fill:#FFB6C1
    style H fill:#FFE4B5
    style K fill:#FFE4B5
    style P fill:#FFE4B5
    style V fill:#FFE4B5
    style AA fill:#FFE4B5
    style CC fill:#FFE4B5
```

## Data Structures Flow

```mermaid
flowchart LR
    A[SampleJSON.json] --> B[RootObject]
    B --> C[Packets List]
    C --> D[Packet Object]
    D --> E[Fields List]
    E --> F[Field Object]
    
    F --> G[FieldName]
    F --> H[BitRange]
    F --> I[Description]
    F --> J[Max/Min]
    F --> K[Values Dictionary]
    
    K --> L[Value Key: String]
    L --> M[ValueInfo Object]
    M --> N[Description]
    M --> O[Display]
    
    P[FwByteData] --> Q[Byte Array]
    Q --> R[Header Byte]
    Q --> S[Payload Bytes]
    
    T[DecoderMethod] --> U[Dictionary]
    U --> V[FieldName: String]
    V --> W[BitMsbLsb Dictionary]
    W --> X[Byte Index: uint]
    X --> Y[BitMsbLsb Object]
    Y --> Z[MSB/LSB Values]

    style A fill:#E6F3FF
    style P fill:#FFE6E6
    style T fill:#E6FFE6
```

## Value Lookup Process

```mermaid
flowchart TD
    A[GetValueInfo Method Called] --> B[Check if Field.Values exists]
    B -->|No| C[Return Empty Description<br/>and Display]
    B -->|Yes| D[Convert itemp to String]
    D --> E{itemp Key exists<br/>in Values Dictionary?}
    E -->|No| F[Return Empty Description<br/>and Display]
    E -->|Yes| G[Get ValueInfo Object<br/>from Dictionary]
    G --> H[Extract Description<br/>from ValueInfo]
    H --> I[Extract Display<br/>from ValueInfo]
    I --> J[Return Description<br/>and Display]

    style A fill:#90EE90
    style C fill:#FFB6C1
    style F fill:#FFB6C1
    style J fill:#90EE90
```

## Bit Decoding Process

```mermaid
flowchart TD
    A[Bit Decoding Process] --> B{Single Byte or<br/>Multiple Bytes?}
    B -->|Single| C[PayloadDecodeShifting]
    B -->|Multiple| D[PayloadDecodeShiftingForMultipleBytes]
    
    C --> E[Extract Bit Range<br/>from Single Byte]
    D --> F[Extract and Combine<br/>Bits from Multiple Bytes]
    
    E --> G[Apply MSB/LSB Masking]
    F --> H[Apply MSB/LSB Masking<br/>for Each Byte]
    
    G --> I[Return Bit Order String<br/>and Decoded Value]
    H --> J[Combine Values<br/>Return Bit Order String<br/>and Decoded Value]
    
    I --> K[Use Decoded Value<br/>for Value Lookup]
    J --> K

    style A fill:#90EE90
    style B fill:#FFE4B5
    style K fill:#90EE90
```

## Sample Data Processing Example

```mermaid
flowchart TD
    A[Sample Data: 0x25, 0x31, 0x42] --> B[Header: 0x25]
    B --> C[Match with ADC Packet<br/>Header: 0x25]
    C --> D[Process Request Field<br/>BitRange: B0-b7:b3]
    D --> E[Extract Bits 7-3<br/>from Byte 0 (0x25)]
    E --> F[Decoded Value: 4<br/>Binary: 00100101<br/>Bits 7-3: 00100 = 4]
    F --> G{Check Values Dictionary<br/>for Key "4"}
    G -->|Found| H[No match in Values<br/>Return empty Description/Display]
    G -->|Not Found| I[Check for Key "5"<br/>for reset operation]
    
    D --> J[Process Parameter Field<br/>BitRange: B0-b2:b0|B1-b7:b0]
    J --> K[Extract Bits 2-0 from B0<br/>and Bits 7-0 from B1]
    K --> L[B0 bits 2-0: 101 = 5<br/>B1 all bits: 0x31 = 49]
    L --> M[Combined Value:<br/>(5) + (49 << 3) = 397]

    style A fill:#E6F3FF
    style F fill:#90EE90
    style M fill:#90EE90
```

## Class Relationships

```mermaid
classDiagram
    class RootObject {
        +string SpecVersion
        +List~Packet~ Packets
    }
    
    class Packet {
        +string PacketName
        +string Header
        +List~Field~ Fields
    }
    
    class Field {
        +string FieldName
        +string BitRange
        +string Description
        +int? Max
        +int? Min
        +Dictionary~string,ValueInfo~ Values
    }
    
    class ValueInfo {
        +string Description
        +string Display
    }
    
    class BitMsbLsb {
        +uint MSB
        +uint LSB
    }
    
    class DataDecoder {
        +DecoderMethod(Packet) Dictionary
        +PayloadDecodeShifting() Tuple
        +PayloadDecodeShiftingForMultipleBytes() Tuple
        +GetValueInfo(Field, byte) Tuple
    }
    
    RootObject --> Packet
    Packet --> Field
    Field --> ValueInfo
    DataDecoder --> BitMsbLsb
    DataDecoder --> Field
```

## Key Features

1. **Header Matching**: The system matches the first byte of `FwByteData` with packet headers in the JSON
2. **Bit Range Parsing**: Complex bit ranges like "B0-b7:b3" and "B0-b2:b0|B1-b7:b0" are parsed and processed
3. **Value Lookup**: Decoded values are matched against the Values dictionary to provide meaningful descriptions
4. **Multi-byte Support**: The system handles both single-byte and multi-byte field extractions
5. **Display Formatting**: Results are formatted for easy reading with packet names, field names, and values

## Sample Output Format

```
Packet Name : Auxiliary Data Control (ADC)
    Payload Name : Request
    Bit Order : [bit order string]
    Decoded Value : 4
    Description : [from Values if available]
    Display : [from Values if available]
    ----------------------------------
    Display String : Auxiliary Data Control (ADC)[0x25] - Request : 4| Parameter : 397
```
